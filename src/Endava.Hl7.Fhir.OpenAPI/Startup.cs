using Endava.Hl7.Fhir.Common.Contracts.Converters;
using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Endava.Hl7.Fhir.Common.Core.Services;
using Endava.Hl7.Fhir.OpenAPI.Configurations;
using Endava.Hl7.Fhir.OpenAPI.Converters;
using Endava.Hl7.Fhir.OpenAPI.Extensions;
using Endava.Hl7.Fhir.OpenAPI.Middleware;
using Endava.Hl7.Fhir.OpenAPI.Services.Helpers;
using Endava.Hl7.Fhir.OpenAPI.Services.Options;
using Endava.Hl7.Fhir.OpenAPI.Services;
using FluentValidation.AspNetCore;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Endava.Hl7.Fhir.OpenAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services required for using options
            services.AddOptions();

            // Add the whole configuration object here
            services.AddSingleton(Configuration);

            // Add health check services
            services.AddHealthChecks();

            RegisterConfigurations(services);
            RegisterServices(services);

            services.AddCorsPolicy("EnableCORS");

            // Adds service API versioning
            services.AddAndConfigureApiVersioning();

            // Adds services for controllers
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true; // To disable the automatic 400 behavior, set the SuppressModelStateInvalidFilter property to true
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[404].Link = "https://httpstatuses.com/404";
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            // Adds Swagger support
            services.AddSwaggerMiddleware();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            // Needed for a ReDoc logo
            const string LOGO_FILE_PATH = "wwwroot/swagger";
            var fileprovider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, LOGO_FILE_PATH));
            var requestPath = new PathString($"/{LOGO_FILE_PATH}");

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = fileprovider,
                RequestPath = requestPath,
            });

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = fileprovider,
                RequestPath = requestPath,
                EnableDirectoryBrowsing = false
            });

            app.UseStaticFiles();

            // Register ReDoc middleware
            app.UseReDocMiddleware(config);

            // Register Swagger and SwaggerUI middleware
            app.UseSwaggerMiddleware(config);

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            // For elevated security, it is recommended to remove this middleware and set your server to only listen on https. 
            // A slightly less secure option would be to redirect http to 400, 505, etc.
            app.UseHttpsRedirection();

            // NOTE** Add logging middleware(s) only when not runnig from integration/unit tests!
            if (!UnitTestDetector.IsRunningFromUnitTest())
            {
                // Adds request/response logging middleware
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

                // Adds middleware for streamlined request logging
                app.UseSerilogRequestLogging(options =>
                {
                    // Customize the message template
                    options.MessageTemplate = "{Host} {Protocol} {RequestMethod} {RequestPath} {EndpointName} {ResponseBody} responded {StatusCode} in {Elapsed} ms";
                    options.EnrichDiagnosticContext = RequestLogHelper.EnrichDiagnosticContext;
                });
            }

            // Adds global error handling middleware
            app.UseApiExceptionHandling();

            // Adds enpoint routing middleware
            app.UseRouting();

            // Adds a CORS middleware
            app.UseCors("EnableCORS");

            app.UseEndpoints(endpoints =>
            {
                //Add health check endpoint
                endpoints.MapHealthChecks("/healthz");
                // Adds enpoints for controller actions without specifyinf any routes
                endpoints.MapControllers();
            });

            // Load Citizenship list from CSV file to be a global available
            InitializeCitizenshipService(app);
        }

        /// <summary>
        /// Load Citizenship list from CSV file
        /// </summary>
        /// <param name="app"></param>
        private static void InitializeCitizenshipService(IApplicationBuilder app)
        {
            var citizenshipService = app.ApplicationServices.GetRequiredService<ICitizenshipService>();
            citizenshipService.Initialize();
        }

        /// <summary>
        /// Register a configuration instances which TOptions will bind against
        /// </summary>
        /// <param name="services"></param>
        protected void RegisterConfigurations(IServiceCollection services)
        {
            services.Configure<FhirOptions>(Configuration.GetSection("FhirOptions"));
            services.Configure<ResourcesOptions>(Configuration.GetSection("ResourcesOptions"));
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.Configure<SwaggerConfig>(Configuration.GetSection(nameof(SwaggerConfig)));
        }

        /// <summary>
        /// Register services and middlewares
        /// </summary>
        /// <param name="services"></param>
        protected virtual void RegisterServices(IServiceCollection services)
        {
            // Register middlewares
            services.AddTransient<ApiExceptionHandlingMiddleware>();
            services.AddTransient<RequestResponseLoggingMiddleware>();

            // Services
            // Services
            services.AddTransient<ICsvConverter, CsvConverter>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IObservationService, ObservationService>();
            services.AddTransient<IMedicationService, MedicationService>();
            services.AddTransient<IOrganizationService, OrganizationService>();

            services.AddSingleton<IFhirService, FhirService>();
            services.AddSingleton<ICitizenshipService, CitizenshipService>();

            // Converters
            services.AddTransient<IConverter<Patient, PatientDetailDto>, PatientToDtoConverter>();
            services.AddTransient<IConverter<IList<Patient>, IList<PatientDetailDto>>, PatientToDtoConverter>();
            services.AddTransient<IConverter<Observation, ObservationDto>, ObservationToDtoCoverter>();
            services.AddTransient<IConverter<IList<Observation>, IList<ObservationDto>>, ObservationToDtoCoverter>();
            services.AddTransient<IConverter<PatientCsv, Patient>, PatientCsvToPatientConverter>();
            services.AddTransient<IConverter<IList<PatientCsv>, IList<Patient>>, PatientCsvToPatientConverter>();
            services.AddTransient<IConverter<PatientDto, Patient>, PatientDtoToPatientConverter>();
            services.AddTransient<IConverter<IList<PatientDto>, IList<Patient>>, PatientDtoToPatientConverter>();
        }
    }
}

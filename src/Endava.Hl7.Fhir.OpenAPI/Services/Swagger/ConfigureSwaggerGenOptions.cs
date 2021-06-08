using Endava.Hl7.Fhir.OpenAPI.Configurations;
using Endava.Hl7.Fhir.OpenAPI.Services.Swagger.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;

namespace Endava.Hl7.Fhir.OpenAPI.Services.Swagger
{
    /// <summary>
    /// Configures the Swagger generation options
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly string _appName;
        private readonly IApiVersionDescriptionProvider _apiProvider;
        private readonly SwaggerConfig _swaggerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerGenOptions"/> class
        /// </summary>
        /// <param name="apiProvider">The <see cref="IApiVersionDescriptionProvider">apiProvider</see> used to generate Swagger documents.</param>
        /// <param name="swaggerConfig"></param>
        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiProvider, IOptions<SwaggerConfig> swaggerConfig)
        {
            _apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
            _swaggerConfig = swaggerConfig.Value;
            _appName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
        }

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // Add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();

            // Add a swagger document for each discovered API version
            // Note: you might choose to skip or document deprecated API versions differently
            foreach (var description in _apiProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));

                // Include Document file
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Provide a custom strategy for generating the unique Id's
                options.CustomSchemaIds(x => x.FullName);
            }
        }

        /// <summary>
        /// Create API version
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = _swaggerConfig.Title,
                Version = description.ApiVersion.ToString(),
                Description = _swaggerConfig.Description,
                Contact = new OpenApiContact
                {
                    Name = _swaggerConfig.ContactName,
                    Email = _swaggerConfig.ContactEmail,
                    Url = new Uri(_swaggerConfig.ContactUrl)
                },
                License = new OpenApiLicense
                {
                    Name = _swaggerConfig.LicenseName,
                    Url = new Uri(_swaggerConfig.LicenseUrl)
                },
                // Add a logo to ReDoc page
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    {
                        "x-logo", new OpenApiObject
                        {
                            {"url", new OpenApiString("/wwwroot/swagger/fhir-logo.png")}
                        }
                    }
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " ** THIS API VERSION HAS BEEN DEPRECATED!";
            }

            return info;
        }
    }
}
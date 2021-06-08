using Endava.Hl7.Fhir.OpenAPI.Services.Authorization;
using Endava.Hl7.Fhir.OpenAPI.Services.Options;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public class FhirService : IFhirService
    {
        private readonly ILogger<FhirService> _logger;
        private readonly FhirOptions _fhirOptions;

        public FhirService(ILogger<FhirService> logger, IOptions<FhirOptions> fhirOptions)
        {
            _logger = logger;
            _fhirOptions = fhirOptions.Value;
        }

        public void Initialize()
        {
            _logger.LogDebug(nameof(Initialize));
            var defSettings = new FhirClientSettings
            {
                UseFormatParameter = true,
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation,
                Timeout = 10000 // 10 seconds
            };
            Endpoint = _fhirOptions.Endpoint;

            if (string.IsNullOrWhiteSpace(_fhirOptions.BearerToken))
            {
                Client = new FhirClient(Endpoint, defSettings);
            }
            else
            {
                var messageHandler = new AuthenticationMessageHandler
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _fhirOptions.BearerToken)
                };
                Client = new FhirClient(Endpoint, defSettings, messageHandler);
            }
        }

        public FhirClient Client { get; set; }

        public string Endpoint { get; set; }
    }
}

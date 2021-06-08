using Endava.Hl7.Fhir.Common.Core.Csv.Mappers;
using Endava.Hl7.Fhir.Common.Core.Csv.Models;
using Endava.Hl7.Fhir.Common.Core.Services;
using Endava.Hl7.Fhir.OpenAPI.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    /// <summary>
    /// Reads citizenship county codes from csv file.
    /// https://dw.opm.gov/datastandards/referenceData/1422/current?index=C
    /// </summary>
    public class CitizenshipService : ICitizenshipService
    {
        private readonly ICsvConverter _csvConverter;
        private readonly ILogger<CitizenshipService> _logger;
        private readonly ResourcesOptions _resourcesOptions;

        public CitizenshipService(ILogger<CitizenshipService> logger, IOptions<ResourcesOptions> resourcesOptions,
            ICsvConverter csvConverter)
        {
            _logger = logger;
            _csvConverter = csvConverter;
            _resourcesOptions = resourcesOptions.Value;
        }

        public Dictionary<string, Citizenship> Citizenships { get; private set; }

        public Citizenship GetCitizenship(string code)
        {
            Citizenship result = null;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }
            if (Citizenships.ContainsKey(code))
            {
                result = Citizenships[code];
            }
            return result;
        }

        public void Initialize()
        {
            _logger.LogDebug(nameof(Initialize));
            var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _resourcesOptions.CitizenshipCsvFilename);
            var records = _csvConverter.ConvertTo<Citizenship, CitizenshipMap>(filepath);
            var result = new Dictionary<string, Citizenship>();
            foreach (var rec in records)
            {
                result.TryAdd(rec.Code, rec);
            }
            Citizenships = result;
        }
    }
}

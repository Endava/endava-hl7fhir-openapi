using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Endava.Hl7.Fhir.Common.Contracts.Converters
{
    /// <summary>
    /// Patient to PatientDto converter
    /// </summary>
    public class PatientToDtoConverter : IConverter<Patient, PatientDetailDto>, IConverter<IList<Patient>, IList<PatientDetailDto>>
    {
        private readonly ILogger<PatientToDtoConverter> _logger;

        public PatientToDtoConverter(ILogger<PatientToDtoConverter> logger)
        {
            _logger = logger;
        }

        public PatientDetailDto Convert(Patient patient)
        {
            _logger.LogDebug(nameof(Convert));

            // BirthPlace Extension
            var birthPlace = string.Empty;
            var birthPlaceExtension = patient.Extension.FirstOrDefault(ext => ext.Url.EndsWith("patient-birthPlace"));
            if (birthPlaceExtension != null)
            {
                birthPlace = ((Address)birthPlaceExtension.Value).City;
            }

            // Citizenship Extension
            var citizenship = string.Empty;
            var citizenshipCode = string.Empty;
            var citizenshipExtension = patient.Extension.FirstOrDefault(ext => ext.Url.EndsWith("patient-citizenship"));
            var codeExt = citizenshipExtension?.Extension.FirstOrDefault(ext => ext.Url.Equals("code"));
            if (codeExt != null)
            {
                var citizenshipConcept = (CodeableConcept)codeExt.Value;
                citizenship = citizenshipConcept.Coding.FirstOrDefault()?.Display;
                citizenshipCode = citizenshipConcept.Coding.FirstOrDefault()?.Code;
            }

            var firstName = patient.Name.FirstOrDefault();
            var firstAddress = patient.Address.FirstOrDefault();

            var address = new AddressDetailDto();
            if (firstAddress != null)
            {
                address.PostalCode = firstAddress.PostalCode;
                address.City = firstAddress.City;
                address.Country = firstAddress.Country;
                address.Line = firstAddress.Line?.ToArray();
                address.Type = firstAddress.Type == Address.AddressType.Both ? "Both" : 
                    firstAddress.Type == Address.AddressType.Physical ? "Physical" : "Postal";
            }

            var gender = "Unknown";
            if (patient.Gender != null)
            {
                gender = patient.Gender == AdministrativeGender.Female ? "Female" :
                    patient.Gender == AdministrativeGender.Male ? "Male" : "Unknown";
            }

            var patientDto = new PatientDetailDto
            {
                Id = patient.Id,
                Identifier = patient.Identifier?.FirstOrDefault()?.Value,
                Prefix = firstName?.Prefix?.FirstOrDefault(),
                FirstName = firstName?.Given.FirstOrDefault(),
                LastName = firstName?.Family,
                BirthDate = patient.BirthDate,
                BirthPlace = birthPlace,
                Citizenship = citizenship,
                CitizenshipCode = citizenshipCode,
                Gender = gender,
                Address = address
            };
            return patientDto;
        }

        public IList<PatientDetailDto> Convert(IList<Patient> patients)
        {
            _logger.LogDebug("ConvertList");
            return patients.Select(cmp =>
            {
                var patientDto = Convert(cmp);
                return patientDto;
            }).ToList();
        }
    }
}

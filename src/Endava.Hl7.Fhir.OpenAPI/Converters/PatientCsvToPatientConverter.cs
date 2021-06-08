using Endava.Hl7.Fhir.Common.Contracts.Converters;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Hl7.Fhir.Model;
using Endava.Hl7.Fhir.OpenAPI.Services;
using Endava.Hl7.Fhir.OpenAPI.Services.Options;
using Hl7.Fhir.Support;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Endava.Hl7.Fhir.OpenAPI.Converters
{
    /// <summary>
    /// Patient to PatientDto converter
    /// </summary>
    public class PatientCsvToPatientConverter : IConverter<PatientCsv, Patient>, IConverter<IList<PatientCsv>, IList<Patient>>
    {
        private readonly ILogger<PatientCsvToPatientConverter> _logger;
        private readonly ICitizenshipService _citizenshipService;
        private readonly FhirOptions _fhirOptions;

        public PatientCsvToPatientConverter(ILogger<PatientCsvToPatientConverter> logger,
            IOptions<FhirOptions> fhirOptions, ICitizenshipService citizenshipService)
        {
            _logger = logger;
            _fhirOptions = fhirOptions.Value;
            _citizenshipService = citizenshipService;
        }

        public Patient Convert(PatientCsv patient)
        {
            _logger.LogDebug(nameof(Convert));

            var id = Guid.NewGuid().ToFhirId();
            var newPatient = new Patient()
            {
                Id = id,
                Active = true,
                Identifier = new List<Identifier> {
                    new()
                    {
                        System = "http://acme.org/patient-ids",
                        Value = patient.Identifier
                    }
                },
                ManagingOrganization = new ResourceReference(_fhirOptions.ManagingOrganization),
                Gender = patient.Gender.ToUpper().Equals("MALE") ? AdministrativeGender.Male :
                    patient.Gender.ToUpper().Equals("FEMALE") ? AdministrativeGender.Female :
                    AdministrativeGender.Unknown,
                Name = new List<HumanName>
                {
                    new()
                    {
                        Prefix = new[] {
                            patient.Prefix
                        },
                        Family = patient.LastName,
                        Given = new[] {
                            patient.FirstName
                        },
                        Use = HumanName.NameUse.Official
                    }
                },
                BirthDate = patient.BirthDate,
                Address = new List<Address>()
                {
                    new()
                    {
                        City = patient.AddressCity,
                        Country = patient.AddressCountry,
                        PostalCode = patient.AddressPostalCode,
                        Line = new[] { $"{patient.AddressStreetName} {patient.AddressStreetNo} {patient.AddressAppartmentNo}" },
                        Type = patient.AddressType.ToUpper().Equals("BOTH") ? Address.AddressType.Both :
                                    patient.AddressType.ToUpper().Equals("PHYSICAL") ? Address.AddressType.Physical :
                                    Address.AddressType.Postal
                    }
                }
            };

            // Add Patient's simple extension "patient-birthPlace"
            // Read more: https://www.hl7.org/fhir/patient-profiles.html
            var birthPlace = new Address()
            {
                City = patient.BirthPlace
            };
            newPatient.Extension.Add(new Extension("http://hl7.org/fhir/StructureDefinition/patient-birthPlace", birthPlace));

            // Check Citizenship
            var citizenship = _citizenshipService.GetCitizenship(patient.CitizenshipCode);
            if (citizenship != null)
            {
                // Convert to DateTime
                var citizenshipStart = DateTime.ParseExact(citizenship.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                // Add Patient's complex extension "patient-citizenship"
                var citizenshipExt = new Extension
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/patient-citizenship",
                    Extension = new List<Extension>()
                    {
                        new("code",
                            new CodeableConcept()
                            {
                                Coding = new List<Coding>
                                {
                                    new()
                                    {
                                        Code = citizenship.Code,
                                        Display = citizenship.Explanation,
                                        System = "urn:iso:std:iso:3166" // https://hl7.org/fhir/2018May/iso3166.html
                                    }
                                },
                            }
                        ),
                        new("period",
                            new Period
                            {
                                Start = citizenshipStart.ToString("yyyy-MM-dd")
                            }
                        )
                    }
                };

                newPatient.Extension.Add(citizenshipExt);
            }

            return newPatient;
        }

        public IList<Patient> Convert(IList<PatientCsv> patients)
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

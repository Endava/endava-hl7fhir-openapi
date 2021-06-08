using CsvHelper;
using Endava.Hl7.Fhir.Common.Contracts.Converters;
using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly ICitizenshipService _citizenshipService;
        private readonly FhirClient _fhirClient;
        private readonly IConverter<PatientCsv, Patient> _patientCsvToPatientConverter;
        private readonly IConverter<PatientDto, Patient> _patientDtoToPatientConverter;

        public PatientService(IFhirService fhirService,
            ICitizenshipService citizenshipService,
            IConverter<PatientCsv, Patient> patientCsvToPatientConverter,
            IConverter<PatientDto, Patient> patientDtoToPatientConverter)
        {
            _citizenshipService = citizenshipService;
            _patientCsvToPatientConverter = patientCsvToPatientConverter;
            _patientDtoToPatientConverter = patientDtoToPatientConverter;
            fhirService.Initialize();
            _fhirClient = fhirService.Client;
        }

        /// <summary>
        /// Create a new Patint
        /// </summary>
        /// <param name="patient"></param>
        /// <returns>New Patient</returns>
        public async Task<Patient> CreatePatientAsync(PatientDto patient)
        {
            Patient result = null;
            var newPatient = _patientDtoToPatientConverter.Convert(patient);
            // Before adding the patient to the server, let's validate it first
            bool success = ValidatePatient(newPatient);
            if (success)
            {
                result = await _fhirClient.CreateAsync(newPatient).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Create patients from CSV list
        /// </summary>
        /// <param name="patientsCsv">List of patients</param>
        /// <returns>Number of created patients</returns>
        public async Task<int> CreatePatientsAsync(IEnumerable<PatientCsv> patientsCsv)
        {
            var patientCsvs = patientsCsv.ToList();
            if (!patientCsvs.Any())
            {
                return 0;
            }
            var builder = new TransactionBuilder(_fhirClient.Endpoint, Bundle.BundleType.Transaction);
            foreach (var patientCsv in patientCsvs.Where(p => !string.IsNullOrWhiteSpace(p.Identifier)))
            {
                var patient = _patientCsvToPatientConverter.Convert(patientCsv);
                builder.Create(patient);
            }
            var response = await _fhirClient.TransactionAsync(builder.ToBundle()).ConfigureAwait(false);
            return response.Entry.Count;
        }

        /// <summary>
        /// Delete Patient by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Success/Fail</returns>
        public async Task<bool> DeletePatientByIdentifierAsync(string identifier)
        {
            var patient = await SearchByIdentifierAsync(identifier).ConfigureAwait(false);
            if (patient == null)
            {
                return false;
            }
            await _fhirClient.DeleteAsync(patient).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Get list of existing Patients
        /// </summary>
        /// <param name="patientsCsv"></param>
        /// <returns>List of existing Patients</returns>
        public async Task<IList<PatientCsv>> ExistingPatientsAsync(IEnumerable<PatientCsv> patientsCsv)
        {
            var result = new List<PatientCsv>();
            foreach (var patientCsv in patientsCsv.Where(p => !string.IsNullOrWhiteSpace(p.Identifier)))
            {
                var patient = await SearchByIdentifierAsync(patientCsv.Identifier).ConfigureAwait(false);
                if (patient != null)
                {
                    result.Add(patientCsv);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>Get Patients page
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>List of Patients</returns>
        public async Task<IEnumerable<Patient>> GetPatientsAsync(int pageSize = 10)
        {
            var result = await _fhirClient.SearchAsync<Patient>(pageSize: pageSize).ConfigureAwait(false);
            return result.Entry.Select(x => (Patient)x.Resource);
        }

        /// <summary>
        /// 
        /// </summary>Get Patients by first and last name
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>List of Patients</returns>
        public async Task<IEnumerable<Patient>> GetPatientsByNameAsync(string firstName, string lastName)
        {
            var searchParams = new SearchParams();
            searchParams.Add("Given", firstName);
            searchParams.Add("Family", lastName);

            var result = await _fhirClient.SearchAsync<Patient>(searchParams).ConfigureAwait(false);
            return result.Entry.Select(x => (Patient)x.Resource);
        }

        /// <summary>
        /// Parse Patients from memory stream
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns>List of CSV Patients</returns>
        public IList<PatientCsv> ParsePatients(byte[] fileContent)
        {
            TextReader reader = new StreamReader(new MemoryStream(fileContent));
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<PatientCsv>().ToList();
            return records;
        }

        /// <summary>
        /// Get Patient by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Patient</returns>
        public async Task<Patient> SearchByIdentifierAsync(string identifier)
        {
            Patient result = null;
            var bundle = await _fhirClient.SearchAsync<Patient>(new[] { $"identifier={identifier}" }).ConfigureAwait(false);
            if (bundle.Entry.Any())
            {
                result = (Patient)bundle.Entry.First().Resource;
            }
            return result;
        }

        /// <summary>
        /// Get Patient by resource Id
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns>Patient</returns>
        public async Task<Patient> SearchByResourceIdAsync(string resourceId)
        {
            Patient result = null;
            var bundle = await _fhirClient.SearchByIdAsync<Patient>(resourceId).ConfigureAwait(false);
            if (bundle.Entry.Any())
            {
                result = (Patient)bundle.Entry.First().Resource;
            }
            return result;
        }

        /// <summary>
        /// Update existing Patient
        /// </summary>
        /// <param name="patientDto"></param>
        /// <returns>Updated Patient</returns>
        public async Task<Patient> UpdatePatientAsync(PatientDto patientDto)
        {
            Patient result = null;
            var patient = await SearchByIdentifierAsync(patientDto.Identifier).ConfigureAwait(false);
            if (patient != null)
            {
                patient.Gender = patientDto.Gender.ToUpper().Equals("MALE") ? AdministrativeGender.Male :
                    patientDto.Gender.ToUpper().Equals("FEMALE") ? AdministrativeGender.Female :
                    AdministrativeGender.Unknown;
                patient.Name = new List<HumanName>
                {
                    new()
                    {
                        Prefix = new[] {
                            patientDto.Prefix
                        },
                        Family = patientDto.LastName,
                        Given = new[] {
                            patientDto.FirstName
                        },
                        Use = HumanName.NameUse.Official
                    }
                };
                patient.BirthDate = patientDto.BirthDate;
                patient.Address = new List<Address>()
                {
                    new()
                    {
                        City = patientDto.Address.City,
                        Country = patientDto.Address.Country,
                        PostalCode = patientDto.Address.PostalCode,
                        Line = new[] { $"{patientDto.Address.StreetName} {patientDto.Address.StreetNo} {patientDto.Address.AppartmentNo}" },
                        Type = patientDto.Address.Type.ToUpper().Equals("BOTH") ? Address.AddressType.Both :
                                    patientDto.Address.Type.ToUpper().Equals("PHYSICAL") ? Address.AddressType.Physical :
                                    Address.AddressType.Postal
                    }
                };

                // BirthPlace
                var birthPlaceExtension = patient.Extension.FirstOrDefault(ext => ext.Url.EndsWith("patient-birthPlace"));
                if (birthPlaceExtension != null)
                {
                    ((Address)birthPlaceExtension.Value).City = patientDto.BirthPlace;
                }

                // Citizenship
                var citizenship = _citizenshipService.GetCitizenship(patientDto.CitizenshipCode);
                if (citizenship != null)
                {
                    var citizenshipExtension = patient.Extension.FirstOrDefault(ext => ext.Url.EndsWith("patient-citizenship"));
                    var codeExt = (citizenshipExtension?.Extension)?.FirstOrDefault(ext => ext.Url.Equals("code"));
                    var citizenshipConcept = (CodeableConcept) codeExt?.Value;
                    if (citizenshipConcept != null)
                    {
                        if (citizenshipConcept.Coding.Any())
                        {
                            citizenshipConcept.Coding.FirstOrDefault().Display = citizenship.Explanation;
                            citizenshipConcept.Coding.FirstOrDefault().Code = citizenship.Code;
                        }
                    }
                }

                // Before updating the patient to the server, let's validate it first
                var success = ValidatePatient(patient);
                if (success)
                {
                    result = await _fhirClient.UpdateAsync(patient).ConfigureAwait(false);
                }
            }
            return result;
        }

        /// <summary>
        /// Update Patient's marital status
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="maritalStatusCode"></param>
        /// <returns>Patient</returns>
        public async Task<Patient> UpdatePatientMaritalStatusAsync(string resourceId, string maritalStatusCode)
        {
            Patient patient = await SearchByResourceIdAsync(resourceId).ConfigureAwait(false);
            if (patient != null)
            {
                patient.MaritalStatus = GetMaritalStatus(maritalStatusCode);
                patient = await _fhirClient.UpdateAsync(patient);
            }
            return patient;
        }

        /// <summary>
        /// Update patients from CSV list
        /// </summary>
        /// <param name="patientsCsv">List of patients</param>
        /// <returns>Number of updated patients</returns>
        public async Task<int> UpdatePatientsAsync(IEnumerable<PatientCsv> patientsCsv)
        {
            var patientCsvs = patientsCsv.ToList();
            if (!patientCsvs.Any())
            {
                return 0;
            }
            var builder = new TransactionBuilder(_fhirClient.Endpoint, Bundle.BundleType.Transaction);
            foreach (var patientCsv in patientCsvs.Where(p => !string.IsNullOrWhiteSpace(p.Identifier)))
            {
                var existingPatient = await SearchByIdentifierAsync(patientCsv.Identifier).ConfigureAwait(false);
                if (existingPatient == null)
                {
                    continue;
                }
                var updatedPatient = _patientCsvToPatientConverter.Convert(patientCsv);
                builder.Update(existingPatient.Id, updatedPatient);
            }
            var response = await _fhirClient.TransactionAsync(builder.ToBundle()).ConfigureAwait(false);
            return response.Entry.Count;
        }

        /// <summary>
        /// Get marital status
        /// </summary>
        /// <param name="maritalStatusCode"></param>
        /// <returns>Marital Status</returns>
        private static CodeableConcept GetMaritalStatus(string maritalStatusCode)
        {
            const string SYSTEM = "http://hl7.org/fhir/R4/valueset-marital-status.html";
            maritalStatusCode = maritalStatusCode.ToUpper();
            CodeableConcept result;
            if (maritalStatusCode.Equals("A"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Annulled");
            }
            else if (maritalStatusCode.Equals("D"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Divorced");
            }
            else if (maritalStatusCode.Equals("I"))
            {
                result = new CodeableConcept("http://hl7.org/fhir/R4/valueset-marital-status.html", maritalStatusCode, "Interlocutory");
            }
            else if (maritalStatusCode.Equals("L"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Legally Separated");
            }
            else if (maritalStatusCode.Equals("M"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Married");
            }
            else if (maritalStatusCode.Equals("P"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Polygamous");
            }
            else if (maritalStatusCode.Equals("S"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Never Married");
            }
            else if (maritalStatusCode.Equals("T"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Domestic partner");
            }
            else if (maritalStatusCode.Equals("U"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Unmarried");
            }
            else if (maritalStatusCode.Equals("W"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Widowed");
            }
            else if (maritalStatusCode.Equals("UNK"))
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Unknown");
            }
            else
            {
                result = new CodeableConcept(SYSTEM, maritalStatusCode, "Unknown");
            }
            return result;
        }

        /// <summary>
        /// Validate Patient
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        private static bool ValidatePatient(Patient patient)
        {
            ICollection<ValidationResult> results = new List<ValidationResult>();
            var success = DotNetAttributeValidation.TryValidate(patient, results, true);
            return success;
        }
    }
}
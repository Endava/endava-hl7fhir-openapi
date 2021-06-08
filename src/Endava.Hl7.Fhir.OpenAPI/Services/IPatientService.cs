using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public interface IPatientService
    {
        Task<Patient> CreatePatientAsync(PatientDto patient);

        Task<int> CreatePatientsAsync(IEnumerable<PatientCsv> patients);

        Task<bool> DeletePatientByIdentifierAsync(string identifier);

        Task<IList<PatientCsv>> ExistingPatientsAsync(IEnumerable<PatientCsv> patientsCsv);

        Task<IEnumerable<Patient>> GetPatientsAsync(int pageSize = 10);

        Task<Patient> SearchByIdentifierAsync(string identifier);

        Task<Patient> SearchByResourceIdAsync(string resourceId);

        Task<IEnumerable<Patient>> GetPatientsByNameAsync(string firstName, string lastName);

        IList<PatientCsv> ParsePatients(byte[] fileContent);

        Task<Patient> UpdatePatientAsync(PatientDto patientDto);

        Task<Patient> UpdatePatientMaritalStatusAsync(string resourceId, string maritalStatus);

        Task<int> UpdatePatientsAsync(IEnumerable<PatientCsv> patientsCsv);
    }
}
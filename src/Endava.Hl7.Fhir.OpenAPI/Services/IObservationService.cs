using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public interface IObservationService
    {
        Task<Observation> AddHemoglobinObservationAsync(string patientId, decimal value);

        Task<Observation> AddRedBloodCellCountObservationAsync(string patientId, decimal value);

        Task<Observation> AddWhiteBloodCellCountObservationAsync(string patientId, decimal value);

        Task<List<Observation>> GetHemoglobinObservationsForPatientAsync(string patientId);

        Task<Observation> GetObservationAsync(string observationId);

        Task<List<Observation>> GetObservationsForPatientAsync(string patientId);

        Task<List<Observation>> GetRedBloodCellCountObservationsForPatientAsync(string patientId);

        Task<List<Observation>> GetWhiteBloodCellCountObservationsForPatientAsync(string patientId);
    }
}
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Hl7.Fhir.Model;
using Endava.Hl7.Fhir.OpenAPI.Services.Options;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public class ObservationService : IObservationService
    {
        private readonly FhirClient _fhirClient;
        private readonly FhirOptions _fhirOptions;

        public ObservationService(IFhirService fhirService, IOptions<FhirOptions> fhirOptions)
        {
            _fhirOptions = fhirOptions.Value;
            fhirService.Initialize();
            _fhirClient = fhirService.Client;
        }

        /// <summary>
        /// Create new hemoglobin observation for a Patient
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <param name="value"></param>
        /// <returns>Added observation</returns>
        public async Task<Observation> AddHemoglobinObservationAsync(string patientId, decimal value)
        {
            var hemoglobin = new Hemoglobin(value);
            var observation = CreateObservation(patientId, hemoglobin, DateTime.Now);
            var result = await _fhirClient.CreateAsync(observation).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Create new red blood cell count observation for a Patient
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <param name="value"></param>
        /// <returns>Added observation</returns>
        public async Task<Observation> AddRedBloodCellCountObservationAsync(string patientId, decimal value)
        {
            var redBloodCellCount = new RedBloodCellCount(value);
            var observation = CreateObservation(patientId, redBloodCellCount, DateTime.Now);
            var result = await _fhirClient.CreateAsync(observation).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Create new white blood cell count observation for a Patient
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <param name="value"></param>
        /// <returns>Added observation</returns>
        public async Task<Observation> AddWhiteBloodCellCountObservationAsync(string patientId, decimal value)
        {
            var redBloodCellCount = new WhiteBloodCellCount(value);
            var observation = CreateObservation(patientId, redBloodCellCount, DateTime.Now);
            var result = await _fhirClient.CreateAsync(observation).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Gets Patient's hemoglobin observations
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <returns>List of hemoglobin observations</returns>
        public async Task<List<Observation>> GetHemoglobinObservationsForPatientAsync(string patientId)
        {
            List<Observation> result = new List<Observation>();
            var searchParameters = new SearchParams();
            searchParameters.Parameters.Add(new Tuple<string, string>("patient", patientId));
            var responsBundle = await _fhirClient.SearchAsync<Observation>(searchParameters).ConfigureAwait(false);
            foreach (var entry in responsBundle.Entry)
            {
                var observation = (Observation)entry.Resource;
                if (observation.Code.Coding.Exists(coding => coding.Code == Hemoglobin.CODE))
                {
                    result.Add(observation);
                }
            }
            return result;
        }

        /// <summary>
        /// Get Patient's single observation
        /// </summary>
        /// <param name="observationId">Observation resource Id</param>
        /// <returns>Observation</returns>
        public async Task<Observation> GetObservationAsync(string observationId)
        {
            var result = await _fhirClient.SearchByIdAsync<Observation>(observationId).ConfigureAwait(false);
            return result.Entry.Select(entry => (Observation)entry.Resource).FirstOrDefault();
        }

        /// <summary>
        /// Gets all Patient's observations
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <returns>List of observations</returns>
        public async Task<List<Observation>> GetObservationsForPatientAsync(string patientId)
        {
            List<Observation> result = new List<Observation>();
            var searchParameters = new SearchParams();
            searchParameters.Parameters.Add(new Tuple<string, string>("patient", patientId));
            var responsBundle = await _fhirClient.SearchAsync<Observation>(searchParameters).ConfigureAwait(false);
            result.AddRange(responsBundle.Entry.Select(entry => (Observation)entry.Resource));
            return result;
        }

        /// <summary>
        /// Gets Patient's red blood cell count observations
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <returns>List of red bloodcell count observations</returns>
        public async Task<List<Observation>> GetRedBloodCellCountObservationsForPatientAsync(string patientId)
        {
            List<Observation> result = new List<Observation>();
            var searchParameters = new SearchParams();
            searchParameters.Parameters.Add(new Tuple<string, string>("patient", patientId));
            var responsBundle = await _fhirClient.SearchAsync<Observation>(searchParameters).ConfigureAwait(false);
            foreach (var entry in responsBundle.Entry)
            {
                var observation = (Observation)entry.Resource;
                if (observation.Code.Coding.Exists(coding => coding.Code == RedBloodCellCount.CODE))
                {
                    result.Add(observation);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets Patient's white blood cell count observations
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <returns>List of white bloodcell count observations</returns>
        public async Task<List<Observation>> GetWhiteBloodCellCountObservationsForPatientAsync(string patientId)
        {
            List<Observation> result = new List<Observation>();
            var searchParameters = new SearchParams();
            searchParameters.Parameters.Add(new Tuple<string, string>("patient", patientId));
            var responsBundle = await _fhirClient.SearchAsync<Observation>(searchParameters).ConfigureAwait(false);
            foreach (var entry in responsBundle.Entry)
            {
                var observation = (Observation)entry.Resource;
                if (observation.Code.Coding.Exists(coding => coding.Code == WhiteBloodCellCount.CODE))
                {
                    result.Add(observation);
                }
            }
            return result;
        }

        /// <summary>
        /// Generic private method for creating observations
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="observation"></param>
        /// <param name="timeStamp"></param>
        /// <param name="seqNumber"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private Observation CreateObservation(string patientId,
            ObservationBase observation,
            DateTimeOffset timeStamp,
            int seqNumber = 1, 
            ObservationStatus status = ObservationStatus.Final)
        {
            Observation hb = new Observation()
            {
                Id = observation.Id
            };

            hb.Code = new CodeableConcept(observation.System, observation.Code, observation.Name);
            hb.Value = new Quantity(observation.Value, observation.Unit);
            hb.Subject = new ResourceReference($"{_fhirOptions.Endpoint}/Patient/{patientId}");
            //hb.Subject = new ResourceReference($"urn:uuid:{patientId}");
            hb.Identifier.Add(new Identifier("http://acme.org/sequence-nos", seqNumber.ToString()));
            hb.Effective = new FhirDateTime(timeStamp);
            hb.Status = status;

            // Optionally add the category
            hb.Category.Add(new CodeableConcept("http://terminology.hl7.org/CodeSystem/observation-category", "laboratory", "Laboratory"));

            return hb;
        }
    }
}

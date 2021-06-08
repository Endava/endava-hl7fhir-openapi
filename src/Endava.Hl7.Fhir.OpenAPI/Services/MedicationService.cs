using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly FhirClient _client;

        public MedicationService(IFhirService fhirService)
        {
            fhirService.Initialize();
            _client = fhirService.Client;
        }

        /// <summary>
        /// Get patient's medications
        /// </summary>
        /// <param name="patientId">Patient id</param>
        /// <returns>List of medications</returns>
        public async Task<List<Medication>> GetMedicationDataForPatientAsync(string patientId)
        {
            List<Medication> result;

            var searchParameters = new SearchParams();
            searchParameters.Parameters.Add(new Tuple<string, string>("patient", patientId));
            try
            {
                // https://fhir.cerner.com/millennium/r4/medications/medication-request/#example
                var searchResultResponse = await _client.SearchAsync<MedicationRequest>(searchParameters);
                result = searchResultResponse.Entry.AsParallel() // As parallel since we are making network requests
                    .Select(entry =>
                    {
                        var medOrders = _client.Read<MedicationRequest>("MedicationRequest/" + entry.Resource.Id);
                        var safeCast = (medOrders?.Medication as ResourceReference)?.Reference;
                        if (string.IsNullOrWhiteSpace(safeCast))
                        {
                            return null;
                        };
                        return _client.Read<Medication>(safeCast);
                    })
                    .Where(med => med != null).ToList();
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten();
            }
            catch (FhirOperationException)
            {
                // Error 404 - no medication orders
                result = new List<Medication>();
            }
            return result;
        }
    }
}
using Endava.Hl7.Fhir.Common.Core.Errors;
using Hl7.Fhir.Model;
using Endava.Hl7.Fhir.OpenAPI.Controllers.Base;
using Endava.Hl7.Fhir.OpenAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Endava.Hl7.Fhir.OpenAPI.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MedicationController : BaseController<MedicationController>
    {
        private readonly IMedicationService _medicationService;

        public MedicationController(IFhirService fhirService, IMedicationService medicationService)
        {
            _medicationService = medicationService;
            fhirService.Initialize();
        }

        /// <summary>
        /// Get Patient's Medications
        /// </summary>
        /// <param name="patientId">Patient resource Id</param>
        /// <returns>Return list of Medications</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Medication>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{patientId}", Name = "GetPatientMedications")]
        public async Task<IActionResult> GetPatientMedicationsAsync(string patientId)
        {
            Logger.LogDebug(nameof(GetPatientMedicationsAsync));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var medications = await _medicationService.GetMedicationDataForPatientAsync(patientId).ConfigureAwait(false);
            if (medications == null)
            {
                return NotFound(new NotFoundError("The patient medications was not found"));
            }
            return Ok(medications);
        }
    }
}

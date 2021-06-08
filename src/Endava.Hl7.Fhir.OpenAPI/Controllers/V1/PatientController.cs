using Endava.Hl7.Fhir.Common.Contracts.Converters;
using FluentValidation;
using Endava.Hl7.Fhir.Common.Contracts.Dto;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using Endava.Hl7.Fhir.Common.Core.Attributes;
using Endava.Hl7.Fhir.Common.Core.Errors;
using Hl7.Fhir.Model;
using Endava.Hl7.Fhir.OpenAPI.Controllers.Base;
using Endava.Hl7.Fhir.OpenAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Endava.Hl7.Fhir.OpenAPI.Controllers.V1
{
    /// <summary>
    /// OpenAPI controller for managing patients
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PatientController : BaseController<PatientController>
    {
        private readonly IPatientService _patientService;
        private readonly IConverter<Patient, PatientDetailDto> _patientToDtoConverter;
        private readonly IConverter<IList<Patient>, IList<PatientDetailDto>> _patientsToDtoConverter;
        private readonly IValidator<IList<PatientCsv>> _patientsCsvValidator;

        public PatientController(IFhirService fhirService, IPatientService patientService,
            IConverter<Patient, PatientDetailDto> patientToDtoConverter,
            IConverter<IList<Patient>, IList<PatientDetailDto>> patientsToDtoConverter,
            IValidator<IList<PatientCsv>> patientsCsvValidator)
        {
            _patientService = patientService;
            _patientToDtoConverter = patientToDtoConverter;
            _patientsToDtoConverter = patientsToDtoConverter;
            _patientsCsvValidator = patientsCsvValidator;
            fhirService.Initialize();
        }

        /// <summary>
        /// Create a new Patient
        /// </summary>
        /// <returns>Return Patient</returns>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientDto patient)
        {
            Logger.LogDebug(nameof(CreatePatientAsync));
            if (ModelState.IsValid)
            {
                var newPatient = await _patientService.CreatePatientAsync(patient).ConfigureAwait(false);
                if (newPatient == null)
                {
                    return NotFound(new NotFoundError("The patient was not created"));
                }
                var result = _patientToDtoConverter.Convert(newPatient);
                return Ok(result);
            }
            else
            {
                var errors = ModelStateErrors();
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Delete Patient by identifier
        /// </summary>
        /// <param name="identifier">Identifier (ex. PAT0001)</param>
        /// <returns>Success</returns>
        [HttpDelete("DeleteByIdentifier/{identifier}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteByIdentifierAsync(string identifier)
        {
            Logger.LogDebug(nameof(DeleteByIdentifierAsync));
            if (ModelState.IsValid)
            {
                var result = await _patientService.DeletePatientByIdentifierAsync(identifier).ConfigureAwait(false);
                if (!result)
                {
                    return NotFound(new NotFoundError($"The Patient {identifier} was not deleted"));
                }
                return Ok($"The Patient {identifier} was deleted");
            }
            return BadRequest();
        }

        /// <summary>
        /// Get Patients
        /// </summary>
        /// <param name="pageSize">Page size, default = 10</param>
        /// <returns>Return Patient</returns>[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("Get/{pageSize}")]
        public async Task<IActionResult> GetAsync(int pageSize = 10)
        {
            Logger.LogDebug(nameof(GetAsync));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var patients = await _patientService.GetPatientsAsync(pageSize).ConfigureAwait(false);
            if (patients == null)
            {
                return NotFound(new NotFoundError("The patients were not found"));
            }
            var result = _patientsToDtoConverter.Convert(patients.ToList());
            return Ok(result);
        }

        /// <summary>
        /// Get Patient by identifier
        /// </summary>
        /// <param name="identifier">Identifier (ex. PAT0001)</param>
        /// <returns>Return Patient</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetByIdentifier/{identifier}")]
        public async Task<IActionResult> GetByIdentifierAsync(string identifier)
        {
            Logger.LogDebug(nameof(GetByIdentifierAsync));
            if (ModelState.IsValid)
            {
                var patient = await _patientService.SearchByIdentifierAsync(identifier).ConfigureAwait(false);
                if (patient == null)
                {
                    return NotFound(new NotFoundError($"The patient {identifier} was not found"));
                }
                var result = _patientToDtoConverter.Convert(patient);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// Get Patient by resource Id
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <returns>Return Patient</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetByResourceId/{resourceId}")]
        public async Task<IActionResult> GetByResourceIdAsync(string resourceId)
        {
            Logger.LogDebug(nameof(GetByResourceIdAsync));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var patient = await _patientService.SearchByResourceIdAsync(resourceId).ConfigureAwait(false);
            if (patient == null)
            {
                return NotFound(new NotFoundError($"The patient {resourceId} was not found"));
            }
            var result = _patientToDtoConverter.Convert(patient);
            return Ok(result);
        }

        /// <summary>
        /// Update Patient Marital Status
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <param name="maritalStatusCode">Marital Status Code</param>
        /// <returns>Return Patient</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{resourceId}/{maritalStatusCode}", Name = "UpdateMaritalStatus")]
        public async Task<ActionResult> UpdateMaritalStatusAsync([Required] string resourceId, [Required] string maritalStatusCode)
        {
            Logger.LogDebug(nameof(UpdateMaritalStatusAsync));
            if (ModelState.IsValid)
            {
                var patient = await _patientService.UpdatePatientMaritalStatusAsync(resourceId, maritalStatusCode).ConfigureAwait(false);
                if (patient == null)
                {
                    return NotFound(new NotFoundError("The patient was not updated"));
                }
                var result = _patientToDtoConverter.Convert(patient);
                return Ok(result);
            }
            else
            {
                var errors = ModelStateErrors();
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Update Patient
        /// </summary>
        /// <param name="patient">Patient Dto</param>
        /// <returns>Return Patient</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("Update")]
        public async Task<ActionResult> UpdatePatientAsync([FromBody] PatientDto patient)
        {
            Logger.LogDebug(nameof(UpdatePatientAsync));
            if (ModelState.IsValid)
            {
                var updatedPatient = await _patientService.UpdatePatientAsync(patient).ConfigureAwait(false);
                if (updatedPatient == null)
                {
                    return NotFound(new NotFoundError("The patient was not updated"));
                }
                var result = _patientToDtoConverter.Convert(updatedPatient);
                return Ok(result);
            }
            else
            {
                var errors = ModelStateErrors();
                return BadRequest(errors);
            }
        }

        /// <summary>
        /// Uploads a CSV file with Patients
        /// </summary>
        /// <returns>Operation status</returns>
        [SwaggerUploadFile("text/CSV")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("Upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPatientsAsync()
        {
            var file = Request.Form.Files[0];
            await using (var fileContentStream = new MemoryStream())
            {
                await file.CopyToAsync(fileContentStream).ConfigureAwait(false);

                // Parse CSV file to a list of models
                var allPatients = _patientService.ParsePatients(fileContentStream.ToArray());

                // Validate input
                var validationResult = await _patientsCsvValidator.ValidateAsync(allPatients);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult);
                }

                var existingPatients = await _patientService.ExistingPatientsAsync(allPatients).ConfigureAwait(false);
                var newPatients = allPatients.AsQueryable().Except(existingPatients).ToList();

                await _patientService.CreatePatientsAsync(newPatients).ConfigureAwait(false);
                await _patientService.UpdatePatientsAsync(existingPatients).ConfigureAwait(false);
            }
            return Ok();
        }
    }
}

using Endava.Hl7.Fhir.Common.Core.Errors;
using Hl7.Fhir.Model;
using Endava.Hl7.Fhir.OpenAPI.Controllers.Base;
using Endava.Hl7.Fhir.OpenAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Endava.Hl7.Fhir.OpenAPI.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [EnableCors("EnableCORS")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrganizationController : BaseController<OrganizationController>
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IFhirService fhirService, IOrganizationService organizationService)
        {
            _organizationService = organizationService;
            fhirService.Initialize();
        }

        /// <summary>
        /// Add new Organization
        /// </summary>
        /// <param name="identifier">Identifier (ex. ORG0001)</param>
        /// <param name="name">Name of Organization</param>
        /// <param name="contactPhone">Contact Phone</param>
        /// <returns>Return added Organization</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Organization))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost(Name = "AddOrganization")]
        public async Task<IActionResult> AddOrganizationAsync([Required] string identifier, [Required] string name, [Required] string contactPhone)
        {
            Logger.LogDebug(nameof(AddOrganizationAsync));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var organization = await _organizationService.AddOrganizationAsync(identifier, name, contactPhone).ConfigureAwait(false);
            if (organization == null)
            {
                return NotFound(new NotFoundError("The Organization was not created"));
            }
            return Ok(organization);
        }

        /// <summary>
        /// Get Organization by identifier
        /// </summary>
        /// <param name="identifier">Identifier (ex. ORG0001)</param>
        /// <returns>Return Organization</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Organization))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("GetByIdentifier/{identifier}")]
        public async Task<IActionResult> GetByIdentifierAsync(string identifier)
        {
            Logger.LogDebug(nameof(GetByIdentifierAsync));
            if (ModelState.IsValid)
            {
                var organization = await _organizationService.SearchByIdentifierAsync(identifier).ConfigureAwait(false);
                if (organization == null)
                {
                    return NotFound(new NotFoundError($"The Organization {identifier} was not found"));
                }
                return Ok(organization);
            }
            return BadRequest();
        }
    }
}

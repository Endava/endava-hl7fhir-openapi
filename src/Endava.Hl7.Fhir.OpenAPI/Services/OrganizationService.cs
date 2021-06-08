using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Support;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Endava.Hl7.Fhir.OpenAPI.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly FhirClient _fhirClient;

        public OrganizationService(IFhirService fhirService)
        {
            fhirService.Initialize();
            _fhirClient = fhirService.Client;
        }

        /// <summary>
        /// Add new Organization to the server
        /// </summary>
        /// <param name="identifier">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="contactPhone">Contact Phone</param>
        /// <returns>Added Organization</returns>
        public async Task<Organization> AddOrganizationAsync(string identifier, string name, string contactPhone)
        {
            var organization = CreateOrganization(identifier, name, contactPhone);
            var result = await _fhirClient.CreateAsync(organization).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Get Organization by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>Organization</returns>
        public async Task<Organization> SearchByIdentifierAsync(string identifier)
        {
            Organization result = null;
            var bundle = await _fhirClient.SearchAsync<Organization>(new[] { $"identifier={identifier}" }).ConfigureAwait(false);
            if (bundle.Entry.Any())
            {
                result = (Organization)bundle.Entry.First().Resource;
            }
            return result;
        }

        /// <summary>
        /// Create Organization
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="name"></param>
        /// <param name="contactPhone"></param>
        /// <returns>New Organization</returns>
        private static Organization CreateOrganization(string identifier, string name, string contactPhone)
        {
            var id = Guid.NewGuid().ToFhirId();
            var organization = new Organization
            {
                Id = id,
                Active = true,
                Identifier =
                {
                    new Identifier("http://acme.org/organization-ids", identifier)
                },
                Name = name
            };
            var contactPoint = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = contactPhone
            };
            organization.Telecom.Add(contactPoint);
            return organization;
        }
    }
}
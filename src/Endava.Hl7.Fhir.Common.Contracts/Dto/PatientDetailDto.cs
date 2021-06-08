using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Dto
{
    /// <summary>
    /// Patient Data Transfer Object
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PatientDetailDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("birthPlace")]
        public string BirthPlace { get; set; }

        [JsonProperty("citizenship")]
        public string Citizenship { get; set; }

        [JsonProperty("citizenshipCode")]
        public string CitizenshipCode { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("address")]
        public AddressDetailDto Address { get; set; }
    }
}
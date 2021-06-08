using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Dto
{
    /// <summary>
    /// Patient Address Data Transfer Object
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class AddressDetailDto
    {
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public string[] Line { get; set; }
    }
}



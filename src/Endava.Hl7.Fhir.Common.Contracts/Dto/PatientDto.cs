using Endava.Hl7.Fhir.Common.Contracts.Models;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Endava.Hl7.Fhir.Common.Contracts.Dto
{
    /// <summary>
    /// Patient Data Transfer Object
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PatientDto : PatientBase
    {
        public AddressDto Address { get; set; }
    }
}
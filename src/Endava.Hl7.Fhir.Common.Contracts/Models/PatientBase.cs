using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    /// <summary>
    /// Patient Data Transfer Object Base
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PatientBase
    {
        [Required]
        public string Identifier { get; set; }

        public string Prefix { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        public string BirthPlace { get; set; }

        public string CitizenshipCode { get; set; }

        public string Gender { get; set; }

        public override string ToString()
        {
            return $"{Identifier} {FirstName} {LastName}";
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ObservationBase
    {
        // Default empty constructor
        public ObservationBase() { }

        /// <summary>
        /// Observation base class
        /// </summary>
        /// <param name="system"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="unit"></param>
        public ObservationBase(string system, string code, string name, string unit)
        {
            System = system;
            Code = code;
            Name = name;
            Unit = unit;
        }

        [Required]
        public string Id { get; set; }

        [Required]
        public string System { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        public string Unit { get; set; }

        public DateTimeOffset Effective { get; set; }
    }
}

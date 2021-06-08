using System;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    /// <summary>
    /// Hemoglobin model
    /// More info: https://loinc.org/718-7/
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class Hemoglobin : ObservationBase
    {
        public const string SYSTEM = "http://loinc.org";
        public const string CODE = "718-7";
        public const string NAME = "Hemoglobin [Mass/volume] in Blood";
        public const string UNIT = "g/dL";
        public const decimal MIN_VALUE = 133.0M; // Normal range - minimum value
        public const decimal MAX_VALUE = 167.0M; // Normal range - maximum value

        public Hemoglobin(decimal value)
            : base(SYSTEM, CODE, NAME, UNIT)
        {
            Id = Guid.NewGuid().ToString();
            Value = value;
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    /// <summary>
    /// RedBloodCellCount model
    /// More info: https://loinc.org/789-8/
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class RedBloodCellCount : ObservationBase
    {
        public const string SYSTEM = "http://loinc.org";
        public const string CODE = "789-8";
        public const string NAME = "Erythrocytes [#/volume] in Blood by Automated count (RBCs)";
        public const string UNIT = "10*6/uL";
        public const decimal MIN_VALUE = 4.5M; // Normal range - minimum value
        public const decimal MAX_VALUE = 5.7M; // Normal range - maximum value

        public RedBloodCellCount(decimal value)
            : base(SYSTEM, CODE, NAME, UNIT)
        {
            Id = Guid.NewGuid().ToString();
            Value = value;
        }
    }
}

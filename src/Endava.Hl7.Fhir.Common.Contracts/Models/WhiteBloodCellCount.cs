using System;
using System.Diagnostics.CodeAnalysis;

namespace Endava.Hl7.Fhir.Common.Contracts.Models
{
    /// <summary>
    /// WhiteBloodCellCount model
    /// More info: https://loinc.org/6690-2/
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class WhiteBloodCellCount : ObservationBase
    {
        public const string SYSTEM = "http://loinc.org";
        public const string CODE = "6690-2";
        public const string NAME = "Leukocytes [#/volume] in Blood by Automated count (WBCs)";
        public const string UNIT = "10*3/uL";
        public const decimal MIN_VALUE = 4.0M; // Normal range - minimum value
        public const decimal MAX_VALUE = 10.0M; // Normal range - maximum value

        public WhiteBloodCellCount(decimal value)
            : base(SYSTEM, CODE, NAME, UNIT)
        {
            Id = Guid.NewGuid().ToString();
            Value = value;
        }
    }
}

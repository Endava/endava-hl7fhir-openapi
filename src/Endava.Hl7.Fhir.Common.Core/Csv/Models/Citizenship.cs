using CsvHelper.Configuration.Attributes;

namespace Endava.Hl7.Fhir.Common.Core.Csv.Models
{
    public class Citizenship
    {
        [Name("Code")]
        public string Code { get; set; }

        [Name("Explanation")]
        public string Explanation { get; set; }

        [Name("From")]
        public string From { get; set; }

        [Name("Through")]
        public string Through { get; set; }
    }
}

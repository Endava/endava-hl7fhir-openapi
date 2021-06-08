using FluentValidation;
using Endava.Hl7.Fhir.Common.Contracts.Models;
using System.Collections.Generic;

namespace Endava.Hl7.Fhir.OpenAPI.Validators
{
    public class PatientsCsvValidator : AbstractValidator<IList<PatientCsv>>
    {
        public PatientsCsvValidator()
        {
            RuleForEach(patient => patient).SetValidator(new PatientCsvValidator());
        }
    }
}

using FluentValidation;
using Endava.Hl7.Fhir.Common.Contracts.Models;

namespace Endava.Hl7.Fhir.OpenAPI.Validators
{
    /// <summary>
    /// PatientCsv model validator
    /// </summary>
    /// <remarks>
    /// NOTE: Not all properties are validated!
    /// </remarks>
    public class PatientCsvValidator : AbstractValidator<PatientCsv>
    {
        public PatientCsvValidator()
        {
            RuleFor(model => model.Nr).NotEmpty();
            RuleFor(model => model.Identifier).NotEmpty();
            RuleFor(model => model.FirstName).NotEmpty();
            RuleFor(model => model.LastName).NotEmpty();
            RuleFor(model => model.BirthDate).NotEmpty();
        }
    }
}

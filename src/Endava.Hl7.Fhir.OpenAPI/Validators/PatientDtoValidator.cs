using FluentValidation;
using Endava.Hl7.Fhir.Common.Contracts.Dto;

namespace Endava.Hl7.Fhir.OpenAPI.Validators
{
    /// <summary>
    /// PatientDto model validator
    /// </summary>
    /// <remarks>
    /// NOTE: Not all properties are validated!
    /// </remarks>
    public class PatientDtoValidator : AbstractValidator<PatientDto>
    {
        public PatientDtoValidator()
        {
            RuleFor(model => model.FirstName).NotEmpty().NotEqual("string");
            RuleFor(model => model.LastName).NotEmpty().NotEqual("string");
        }
    }
}

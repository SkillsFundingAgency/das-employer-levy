using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Application.Validation;

namespace SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference
{
    public class CreatePayeSchemeReferenceValidator : IValidator<CreatePayeSchemeCommand>
    {
        public ValidationResult Validate(CreatePayeSchemeCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreatePayeSchemeCommand item)
        {
            throw new NotImplementedException();
        }
    }
}

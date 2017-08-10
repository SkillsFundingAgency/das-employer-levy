using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Application.Validation;

namespace SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference
{
    public class DeletePayeSchemeValidator : IValidator<DeletePayeSchemeCommand>
    {
        public ValidationResult Validate(DeletePayeSchemeCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(DeletePayeSchemeCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Application.Validation;

namespace SFA.DAS.EmployerLevy.Application.Commands.CreateEnglishFractionCalculationDate
{
    public class CreateEnglishFractionCalculationDateCommandValidator : IValidator<CreateEnglishFractionCalculationDateCommand>
    {
        public ValidationResult Validate(CreateEnglishFractionCalculationDateCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.DateCalculated == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.DateCalculated));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateEnglishFractionCalculationDateCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Application.Validation;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationValidator : IValidator<GetLastLevyDeclarationQuery>
    {
        public ValidationResult Validate(GetLastLevyDeclarationQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetLastLevyDeclarationQuery item)
        {
            throw new System.NotImplementedException();
        }
    }
}
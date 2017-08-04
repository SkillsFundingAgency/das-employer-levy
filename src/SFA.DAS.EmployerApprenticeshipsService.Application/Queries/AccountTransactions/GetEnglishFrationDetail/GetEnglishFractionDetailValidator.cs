using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Application.Validation;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailValidator : IValidator<GetEnglishFractionDetailByEmpRefQuery>
    {
        public ValidationResult Validate(GetEnglishFractionDetailByEmpRefQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.EmpRef))
            {
                validationResult.AddError(nameof(item.EmpRef));
            }
           
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetEnglishFractionDetailByEmpRefQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
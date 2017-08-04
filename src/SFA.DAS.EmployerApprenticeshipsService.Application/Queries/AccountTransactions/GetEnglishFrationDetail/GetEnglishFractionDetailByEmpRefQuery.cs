using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailByEmpRefQuery : IAsyncRequest<GetEnglishFractionDetailResposne>
    {
        public string EmpRef { get; set; }
    }
}
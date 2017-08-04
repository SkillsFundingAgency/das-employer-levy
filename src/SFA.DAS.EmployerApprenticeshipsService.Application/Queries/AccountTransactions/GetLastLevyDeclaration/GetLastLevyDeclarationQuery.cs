using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationQuery : IAsyncRequest<GetLastLevyDeclarationResponse>
    {
        public string EmpRef { get; set; }
    }
}

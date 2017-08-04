using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetLastLevyDeclaration
{
    public class GetLastLevyDeclarationResponse
    {
        public DasDeclaration Transaction { get; set; }
    }
}
using SFA.DAS.EAS.Account.Api.Types.Events.Levy;

namespace SFA.DAS.EmployerLevy.Application.Factories
{
    public interface ILevyEventFactory
    {
        LevyDeclarationUpdatedEvent CreateDeclarationUpdatedEvent(string hashedAccountId, string payrollYear, short? payrollMonth);
    }
}

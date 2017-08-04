using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Domain.Models.Audit;

namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IAuditService
    {
        Task SendAuditMessage(EasAuditMessage message);
    }
}
using MediatR;
using SFA.DAS.EmployerLevy.Domain.Models.Audit;

namespace SFA.DAS.EmployerLevy.Application.Commands.AuditCommand
{
    public class CreateAuditCommand : IAsyncRequest
    {
        public EasAuditMessage EasAuditMessage { get; set; }
    }
}

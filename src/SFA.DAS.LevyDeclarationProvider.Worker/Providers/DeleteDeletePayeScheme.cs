using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public class DeleteDeletePayeScheme : Provider<PayeSchemeDeletedMessage>
    {
        [QueueName("employer_shared")]
        public string delete_paye_scheme { get; set; }

        private readonly IMediator _mediator;
        
        public DeleteDeletePayeScheme(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILog log) : base(pollingMessageReceiver, log)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(PayeSchemeDeletedMessage messageContent)
        {
            var empRef = messageContent.EmpRef;
            await _mediator.SendAsync(new DeletePayeSchemeCommand { EmpRef = empRef });
            Log.Info($"Completed removing scheme {empRef}");
        }
    }
}
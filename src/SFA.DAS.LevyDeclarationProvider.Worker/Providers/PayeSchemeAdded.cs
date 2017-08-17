using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public class PayeSchemeAdded : Provider<PayeSchemeCreatedMessage>
    {
        [QueueName("employer_shared")]
        public string add_paye_scheme { get; set; }

        private readonly IMediator _mediator;
        
        public PayeSchemeAdded(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILog logger) : base(pollingMessageReceiver, logger)
        {
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(PayeSchemeCreatedMessage messageContent)
        {
            var empRef = messageContent.EmpRef;
            await _mediator.SendAsync(new CreatePayeSchemeCommand { EmpRef = empRef });
            Log.Info($"Completed added scheme {empRef}");
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public class PayeSchemeAdded : IPayeSchemeAdded
    {
        [QueueName]
        public string add_paye_scheme { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public PayeSchemeAdded(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILogger logger)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<AddPayeSchemeMessage>();
                try
                {
                    if (message?.Content == null)
                    {
                        if (message != null)
                        {
                            await message.CompleteAsync();
                        }
                        continue;
                    }

                    var empRef = message.Content.EmpRef;
                    await _mediator.SendAsync(new CreatePayeSchemeCommand { EmpRef = empRef });

                    await message.CompleteAsync();

                    _logger.Info($"Completed added scheme {empRef}");
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex,$"Failed to add reference to scheme [{message?.Content?.EmpRef}]");
                    break;
                }
                
            }
            
        }
    }
}
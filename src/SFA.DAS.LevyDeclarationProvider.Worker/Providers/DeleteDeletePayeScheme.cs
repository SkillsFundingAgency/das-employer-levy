using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public class DeleteDeletePayeScheme : IDeletePayeScheme
    {
        [QueueName("employer_shared")]
        public string delete_paye_scheme { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILog _log;

        public DeleteDeletePayeScheme(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILog log)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _log = log;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _log.Info("Started Delete Paye Scheme Processing");
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<PayeSchemeDeletedMessage>();
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
                    await _mediator.SendAsync(new DeletePayeSchemeCommand { EmpRef = empRef });

                    await message.CompleteAsync();

                    _log.Info($"Completed removing scheme {empRef}");
                }
                catch (Exception ex)
                {
                    _log.Fatal(ex, $"Failed to remove reference to scheme [{message?.Content?.EmpRef}]");
                    break;
                }

            }
        }
    }
}
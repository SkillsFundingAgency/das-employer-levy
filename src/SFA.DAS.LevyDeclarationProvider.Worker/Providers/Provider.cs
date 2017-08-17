using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public abstract class Provider<T> : IProvider where T : new()
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        protected readonly ILog Log;

        internal Provider(IPollingMessageReceiver pollingMessageReceiver, ILog log)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            Log = log;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<T>();
                try
                {
                    if (message == null)
                    {
                        continue;
                    }

                    if (message.Content == null)
                    {
                        await message.CompleteAsync();
                        continue;
                    }

                    await ProcessMessage(message.Content);

                    await message.CompleteAsync();

                    Log.Info($"Completed message {typeof(T).FullName}");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, $"Failed to process message {typeof(T).FullName}");
                }

            }
        }

        protected abstract Task ProcessMessage(T messageContent);
    }
}

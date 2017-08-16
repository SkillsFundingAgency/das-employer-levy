using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Infrastructure.DependencyResolution;
using SFA.DAS.EmployerLevy.Infrastructure.Logging;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.DependencyResolution;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            Trace.TraceInformation("SFA.DAS.EmployerLevy.Worker is running");

            try
            {
                var levyDeclaration = _container.GetInstance<ILevyDeclaration>();
                var payeSchemeAdded = _container.GetInstance<IPayeSchemeAdded>();
                var deletePayeScheme = _container.GetInstance<IDeletePayeScheme>();

                var taskList = new List<Task>
                {
                    Task.Factory.StartNew(() => levyDeclaration.RunAsync(_cancellationTokenSource.Token)),
                    Task.Factory.StartNew(() => payeSchemeAdded.RunAsync(_cancellationTokenSource.Token)),
                    Task.Factory.StartNew(() => deletePayeScheme.RunAsync(_cancellationTokenSource.Token))
                };
                
                Task.WaitAll(taskList.ToArray());
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            _container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerLevyConfiguration>("SFA.DAS.EmployerLevy"));
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient"));
                c.Policies.Add(new MessagePolicy<EmployerLevyConfiguration>("SFA.DAS.EmployerLevy"));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry<DefaultRegistry>();
            });

            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker is stopping");

            this._cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has stopped");
        }
    }
}

using SFA.DAS.EmployerLevy.Infrastructure.Logging;
using SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.DependencyResolution;
using SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.Updater;

namespace SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggingConfig.ConfigureLogging();

            var container = IoC.Initialize();

            var updater = container.GetInstance<IAccountUpdater>();

            updater.RunUpdate().Wait();
        }
    }
}

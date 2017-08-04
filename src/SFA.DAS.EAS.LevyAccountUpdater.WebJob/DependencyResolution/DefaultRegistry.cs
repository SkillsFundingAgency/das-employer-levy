using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.Updater;
using SFA.DAS.NLog.Logger;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            For<IAccountUpdater>().Use<AccountUpdater>();

            RegisterLogger();
        }

        private void RegisterLogger()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null,
                null)).AlwaysUnique();
        }
    }
}

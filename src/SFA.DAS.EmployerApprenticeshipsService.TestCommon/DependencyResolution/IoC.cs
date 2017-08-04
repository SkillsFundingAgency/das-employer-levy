using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.Infrastructure.DependencyResolution;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.EmployerLevy.TestCommon.DependencyResolution
{
    public static class IoC
    {
        public static Container CreateLevyWorkerContainer(IMessagePublisher messagePublisher, IPollingMessageReceiver messageReceiver, IHmrcService hmrcService, IEventsApi eventsApi = null)
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient"));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry(new LevyWorkerDefaultRegistry(messagePublisher, messageReceiver, hmrcService, eventsApi));
            });
        }
    }
}

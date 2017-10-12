using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Moq;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.Infrastructure.Services;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using StructureMap;
using StructureMap.Graph;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.EmployerLevy.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.EmployerLevy.TestCommon.DependencyResolution
{
    public class LevyWorkerDefaultRegistry : Registry
    {
        public LevyWorkerDefaultRegistry(IMessagePublisher messagePublisher, IPollingMessageReceiver messageReceiver, IHmrcService hmrcService,  IEventsApi eventApi = null)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IConfiguration>().Use<EmployerLevyConfiguration>();
            For<IEventsApi>().Use(eventApi ?? Mock.Of<IEventsApi>()); 
            For<ILog>().Use(Mock.Of<ILog>());
            For<IMessagePublisher>().Use(messagePublisher);
            For<IPollingMessageReceiver>().Use(messageReceiver);
            For<IHmrcService>().Use(hmrcService);
            For<IHmrcDateService>().Use<HmrcDateService>();
            For<IMessageProcessor>().Use<LevyDeclaration>();

            RegisterExecutionPolicies();

            RegisterMapper();

            AddMediatrRegistrations();
        }

        private void RegisterExecutionPolicies()
        {
            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.HmrcExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.HmrcExecutionPolicy.Name);
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

        private void RegisterMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EmployerLevy.Infrastructure").GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                profiles.ForEach(cfg.AddProfile);
            });

            var mapper = config.CreateMapper();

            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(mapper).Singleton();
        }
    }
}

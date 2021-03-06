﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.FileSystem;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EmployerLevy.Infrastructure.DependencyResolution
{
    public class MessagePolicy<T> : ConfiguredInstancePolicy where T : IConfiguration
    {
        private readonly string _serviceName;

        public MessagePolicy(string serviceName)
        {
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher) || x.ParameterType == typeof(IPollingMessageReceiver));

            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            if (messagePublisher != null)
            {
                var queueName = instance.SettableProperties()
                                .FirstOrDefault(c=>c.CustomAttributes.FirstOrDefault(
                                                    x=>x.AttributeType.Name == nameof(QueueNameAttribute)) != null);

                if (queueName != null)
                {
                    var configurationService = new ConfigurationService(GetConfigurationRepository(), new ConfigurationOptions(_serviceName, environment, "1.0"));

                    var config = configurationService.Get<T>();
                    var serviceBusConnectionString = "";

                    if (queueName.CustomAttributes?.FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value != null)
                    {
                        var connectionKey = queueName.CustomAttributes?.FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value.ToString();
                        if (!string.IsNullOrWhiteSpace(connectionKey))
                        {
                            serviceBusConnectionString = config.ServiceBusConnectionStrings[connectionKey];
                        }
                    }


                    if (string.IsNullOrEmpty(serviceBusConnectionString))
                    {
                        var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new FileSystemMessageService(Path.Combine(queueFolder, queueName.Name)));
                    }
                    else
                    {

                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new AzureServiceBusMessageService(serviceBusConnectionString, queueName.Name));
                    }
                }
            }
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
        
    }
}
using System.Collections.Generic;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.EmployerLevy.Domain.Configuration
{
    public class EventsApiClientConfiguration : IEventsApiClientConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
        public Dictionary<string, string> ServiceBusConnectionStrings { get; set; }
    }
}
using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Domain.Configuration
{
    public class EmployerLevyConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string Hashstring { get; set; }
        public EventsApiClientConfiguration EventsApi { get; set; }
    }
}
using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Domain.Configuration
{
    public class EmployerApprenticeshipsServiceConfiguration : IConfiguration
    {
        public string ServiceBusConnectionString { get; set; }
        public IdentityServerConfiguration Identity { get; set; }
        public string DashboardUrl { get; set; }
        public HmrcConfiguration Hmrc { get; set; }
        public string DatabaseConnectionString { get; set; }

        public EventsApiClientConfiguration EventsApi { get; set; }
        
        public string Hashstring { get; set; }
    }
}
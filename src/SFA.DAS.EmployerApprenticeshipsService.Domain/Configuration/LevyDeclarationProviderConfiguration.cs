using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Domain.Configuration
{
    public class LevyDeclarationProviderConfiguration : IConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
using System.Collections.Generic;

namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IConfiguration
    {
        string DatabaseConnectionString { get; set; }
        Dictionary<string, string> ServiceBusConnectionStrings { get; }
    }
}
﻿using System.Collections.Generic;
using SFA.DAS.Audit.Client;
using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Domain.Configuration
{
    public class AuditApiClientConfiguration : IAuditApiConfiguration, IConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string DatabaseConnectionString { get; set; }
        public Dictionary<string, string> ServiceBusConnectionStrings { get; set; }
    }
}
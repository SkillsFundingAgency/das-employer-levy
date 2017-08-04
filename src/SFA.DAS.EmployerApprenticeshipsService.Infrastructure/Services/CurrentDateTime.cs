using System;
using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Infrastructure.Services
{
    public sealed class CurrentDateTime : ICurrentDateTime
    {
        public DateTime Now { get; }

        public CurrentDateTime()
        {
            Now = DateTime.UtcNow;
        }

        public CurrentDateTime(DateTime time)
        {
            Now = time;
        }
    }
}

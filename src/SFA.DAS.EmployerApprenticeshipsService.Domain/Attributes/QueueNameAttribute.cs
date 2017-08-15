using System;

namespace SFA.DAS.EmployerLevy.Domain.Attributes
{
    public class QueueNameAttribute : Attribute
    {
        public QueueNameAttribute(string connectionKey)
        {
        }
    }
}

using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerLevy.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommand : IAsyncRequest
    {
        public List<EmployerLevyData> EmployerLevyData { get; set; }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasLevyService
    {
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);
    }
}
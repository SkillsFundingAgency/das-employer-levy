using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IPayeSchemesRepository
    {
        Task<List<string>> GetPayeSchemes();
    }
}

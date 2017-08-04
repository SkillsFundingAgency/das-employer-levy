using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.Domain.Data.Repositories
{
    public interface IPayeSchemesRepository
    {
        Task<List<string>> GetPayeSchemes();
    }
}

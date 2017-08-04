using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IEmpRefFileBasedService
    {
        Task<string> GetEmpRef(string email, string id);
    }
}

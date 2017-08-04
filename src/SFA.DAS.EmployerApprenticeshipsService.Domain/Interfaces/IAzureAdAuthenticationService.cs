using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IAzureAdAuthenticationService
    {
        Task<string> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    public interface IProvider
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}

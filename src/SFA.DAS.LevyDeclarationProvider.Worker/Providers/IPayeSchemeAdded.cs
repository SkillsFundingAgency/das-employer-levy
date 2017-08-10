using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    internal interface IPayeSchemeAdded
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
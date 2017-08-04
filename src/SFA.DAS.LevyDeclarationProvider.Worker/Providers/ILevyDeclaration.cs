using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers
{
    internal interface ILevyDeclaration
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
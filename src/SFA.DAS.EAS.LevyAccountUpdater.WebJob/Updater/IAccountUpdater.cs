using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.LevyAccountUpdater.WebJob.Updater
{
    public interface IAccountUpdater
    {
        Task RunUpdate();
    }
}
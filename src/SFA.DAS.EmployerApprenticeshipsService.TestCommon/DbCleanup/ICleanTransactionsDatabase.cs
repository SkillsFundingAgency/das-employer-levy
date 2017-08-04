using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.TestCommon.DbCleanup
{
    public interface ICleanTransactionsDatabase
    {
        Task Execute();
    }
}
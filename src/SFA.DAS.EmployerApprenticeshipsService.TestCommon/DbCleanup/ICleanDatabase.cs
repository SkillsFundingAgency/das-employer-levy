using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.TestCommon.DbCleanup
{
    public interface ICleanDatabase
    {
        Task Execute();
    }
}
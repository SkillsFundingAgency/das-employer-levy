using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerLevy.TestCommon.DbCleanup
{
    public interface IUpdateTransactionLine
    {
        Task Execute(long submissionId, DateTime createdDate);
    }
}
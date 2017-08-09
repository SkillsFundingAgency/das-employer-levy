using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerLevy.TestCommon.DbCleanup
{
    public class UpdateTransactionLine : BaseRepository, IUpdateTransactionLine
    {
        public UpdateTransactionLine(EmployerLevyConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task Execute(long submissionId, DateTime createdDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@submissionId", submissionId, DbType.Int64);
            parameters.Add("@createdDate", createdDate, DbType.DateTime);

            await WithConnection(async c => await c.ExecuteAsync(
                "[employer_levy].[UpdateTransactionLineDate_BySubmissionId]",
                parameters,
                commandType: CommandType.StoredProcedure));
        }
    }
}
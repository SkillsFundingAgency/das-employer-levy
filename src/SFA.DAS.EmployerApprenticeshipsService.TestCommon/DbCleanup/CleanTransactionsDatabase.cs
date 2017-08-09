using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerLevy.TestCommon.DbCleanup
{
    public class CleanTransactionsDatabase : BaseRepository, ICleanTransactionsDatabase
    {
        public CleanTransactionsDatabase(EmployerLevyConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task Execute()
        {

            var parameters = new DynamicParameters();
            parameters.Add("@INCLUDETOPUPTABLE", 1, DbType.Int16);
            await WithConnection(async c => await c.ExecuteAsync(
                "[employer_levy].[Cleardown]",
                parameters,
                commandType: CommandType.StoredProcedure));
            
        }
    }
}

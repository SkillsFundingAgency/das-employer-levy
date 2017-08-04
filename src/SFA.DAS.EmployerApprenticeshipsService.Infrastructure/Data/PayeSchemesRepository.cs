using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class PayeSchemesRepository : BaseRepository, IPayeSchemesRepository
    {
        public PayeSchemesRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<string>> GetPayeSchemes()
        {
            var result = await WithConnection(async c =>
            {
                return await c.QueryAsync<string>(
                    sql: "[employer_levy].[GetPayeSchemes]",
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }
    }
}


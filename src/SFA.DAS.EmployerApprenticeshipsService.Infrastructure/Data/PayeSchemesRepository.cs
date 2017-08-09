using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerLevy.Infrastructure.Data
{
    public class PayeSchemesRepository : BaseRepository, IPayeSchemesRepository
    {
        public PayeSchemesRepository(EmployerLevyConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<string>> GetPayeSchemes()
        {
            var result = await WithConnection(async c =>
            {
                return await c.QueryAsync<string>(
                    sql: "[GetPayeSchemes]",
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }
    }
}


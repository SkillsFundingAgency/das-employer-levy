using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerLevy.DataAccess
{
    public class EnglishFractionRepository : BaseRepository, IEnglishFractionRepository
    {
        public EnglishFractionRepository(EmployerLevyConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }
        
        public async Task<DateTime> GetLastUpdateDate()
        {
            var result = await WithConnection(async c => await c.QueryAsync<DateTime>(
                sql: "SELECT Top(1) DateCalculated FROM [EnglishFractionCalculationDate] ORDER BY DateCalculated DESC;",
                commandType: CommandType.Text));

            return Enumerable.FirstOrDefault<DateTime>(result);
        }

        public async Task SetLastUpdateDate(DateTime dateUpdated)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@dateCalculated", dateUpdated, DbType.Date);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [EnglishFractionCalculationDate] (DateCalculated) VALUES (@dateCalculated);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", employerReference, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "SELECT * FROM [EnglishFraction] WHERE EmpRef = @empRef ORDER BY DateCalculated desc;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result;
        }

        public async Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpRef", employerReference, DbType.String);
                parameters.Add("@Amount", fractions.Amount, DbType.Decimal);
                parameters.Add("@dateCalculated", fractions.DateCalculated, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [EnglishFraction] (EmpRef, DateCalculated, Amount) VALUES (@empRef, @dateCalculated, @amount);",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}


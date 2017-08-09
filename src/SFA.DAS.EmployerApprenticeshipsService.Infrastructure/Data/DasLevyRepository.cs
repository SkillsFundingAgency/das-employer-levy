using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerLevy.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        private readonly EmployerLevyConfiguration _configuration;


        public DasLevyRepository(EmployerLevyConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _configuration = configuration;
        }

        public async Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.String);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "SELECT LevyDueYtd, SubmissionId AS Id, SubmissionDate AS [Date] FROM [LevyDeclaration] WHERE empRef = @EmpRef and SubmissionId = @Id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<long>> GetEmployerDeclarationSubmissionIds(string empRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@empRef", empRef, DbType.String);

            var result = await WithConnection(async c => await c.QueryAsync<long>(
                sql: "[GetLevyDeclarationSubmissionIdsByEmpRef]",
                param: parameters,
                commandType: CommandType.StoredProcedure));

            return result.ToList();
        }

        public async Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> declarations, string empRef)
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                using (var unitOfWork = new UnitOfWork(connection))
                {
                    try
                    {
                        foreach (var dasDeclaration in declarations)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@LevyDueYtd", dasDeclaration.LevyDueYtd, DbType.Decimal);
                            parameters.Add("@LevyAllowanceForYear", dasDeclaration.LevyAllowanceForFullYear, DbType.Decimal);
                            parameters.Add("@EmpRef", empRef, DbType.String);
                            parameters.Add("@PayrollYear", dasDeclaration.PayrollYear, DbType.String);
                            parameters.Add("@PayrollMonth", dasDeclaration.PayrollMonth, DbType.Int16);
                            parameters.Add("@SubmissionDate", dasDeclaration.SubmissionDate, DbType.DateTime);
                            parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.Int64);
                            parameters.Add("@HmrcSubmissionId", dasDeclaration.SubmissionId, DbType.Int64);
                            parameters.Add("@CreatedDate", DateTime.UtcNow, DbType.DateTime);
                            if (dasDeclaration.DateCeased.HasValue && dasDeclaration.DateCeased != DateTime.MinValue)
                            {
                                parameters.Add("@DateCeased", dasDeclaration.DateCeased, DbType.DateTime);
                            }
                            if (dasDeclaration.InactiveFrom.HasValue &&
                                dasDeclaration.InactiveFrom != DateTime.MinValue)
                            {
                                parameters.Add("@InactiveFrom", dasDeclaration.InactiveFrom, DbType.DateTime);
                            }
                            if (dasDeclaration.InactiveTo.HasValue && dasDeclaration.InactiveTo != DateTime.MinValue)
                            {
                                parameters.Add("@InactiveTo", dasDeclaration.InactiveTo, DbType.DateTime);
                            }

                            parameters.Add("@EndOfYearAdjustment", dasDeclaration.EndOfYearAdjustment, DbType.Boolean);
                            parameters.Add("@EndOfYearAdjustmentAmount", dasDeclaration.EndOfYearAdjustmentAmount,
                                DbType.Decimal);

                            await unitOfWork.Execute("[CreateDeclaration]", parameters,
                                CommandType.StoredProcedure);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.RollbackChanges();
                        throw;
                    }
                }
            }
        }

        public async Task<DasDeclaration> GetLastSubmissionForScheme(string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "[GetLastLevyDeclarations_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Int32);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }
        
        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "[GetEnglishFraction_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        public async Task ProcessTopupsForScheme(string empRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@empRef", empRef, DbType.String);

            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[CreateTopUpForScheme]",
                param: parameters,
                commandType: CommandType.StoredProcedure));
        }
        
    }
}


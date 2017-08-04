﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task<IEnumerable<long>> GetEmployerDeclarationSubmissionIds(string empRef);
        Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> dasDeclaration, string empRef);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
        Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth);
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);
    }
}

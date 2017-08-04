using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Domain.Data.Repositories
{
    public interface IEnglishFractionRepository
    {
        Task<DateTime> GetLastUpdateDate();
        Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference);
        Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference);
        Task SetLastUpdateDate(DateTime dateUpdated);
    }
}

using System.Collections.Generic;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailResposne
    {
        public IEnumerable<DasEnglishFraction> FractionDetail { get; set; }
    }
}
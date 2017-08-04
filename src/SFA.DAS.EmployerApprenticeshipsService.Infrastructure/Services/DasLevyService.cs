using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetEnglishFrationDetail;
using SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetLastLevyDeclaration;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Infrastructure.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;

        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef)
        {
            var result = await _mediator.SendAsync(new GetEnglishFractionDetailByEmpRefQuery
            {
                EmpRef = empRef
            });

            return result.FractionDetail;
        }

        public async Task<DasDeclaration> GetLastLevyDeclarationforEmpRef(string empRef)
        {
            var result = await _mediator.SendAsync(new GetLastLevyDeclarationQuery
            {
                EmpRef = empRef
            });

            return result.Transaction;
        }
    }
}
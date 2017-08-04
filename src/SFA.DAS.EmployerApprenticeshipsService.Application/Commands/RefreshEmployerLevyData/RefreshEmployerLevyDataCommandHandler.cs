using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IHmrcDateService _hmrcDateService;
        
        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IHmrcDateService hmrcDateService)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _hmrcDateService = hmrcDateService;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var savedDeclarations = new List<DasDeclaration>();
            var updatedEmpRefs = new List<string>();

            foreach (var employerLevyData in message.EmployerLevyData)
            {
                var declarations = employerLevyData.Declarations.Declarations.OrderBy(c => c.SubmissionDate).ToArray();

                declarations = await FilterActiveDeclarations(employerLevyData, declarations);

                ProcessNoPaymentForPeriodDeclarations(declarations, employerLevyData);

                await ProcessEndOfYearAdjustmentDeclarations(declarations, employerLevyData);

                if (!declarations.Any()) continue;

                await _dasLevyRepository.CreateEmployerDeclarations(declarations, employerLevyData.EmpRef);

                updatedEmpRefs.Add(employerLevyData.EmpRef);
                savedDeclarations.AddRange(declarations);
            }

            // TODO: This needs to be published as a LevyDeclarationCreated message which transactions and rds can use
            /* if (savedDeclarations.Any())
            {
                await PublishProcessDeclarationEvents(message, updatedEmpRefs);
                await PublishDeclarationUpdatedEvents(message.AccountId, savedDeclarations);
            } */
        }

        private async Task ProcessEndOfYearAdjustmentDeclarations(IEnumerable<DasDeclaration> declarations, EmployerLevyData employerLevyData)
        {
            var endOfYearAdjustmentDeclarations = declarations.Where(IsEndOfYearAdjustment).ToList();

            foreach (var dasDeclaration in endOfYearAdjustmentDeclarations)
            {
                await UpdateEndOfYearAdjustment(employerLevyData, dasDeclaration);
            }
        }

        private static void ProcessNoPaymentForPeriodDeclarations(IEnumerable<DasDeclaration> declarations, EmployerLevyData employerLevyData)
        {
            var noPaymentForPeriodDeclarations = declarations.Where(x => x.NoPaymentForPeriod);

            foreach (var dasDeclaration in noPaymentForPeriodDeclarations)
            {
                dasDeclaration.LevyDueYtd = null;
            }
        }

        private async Task<DasDeclaration[]> FilterActiveDeclarations(EmployerLevyData employerLevyData, IEnumerable<DasDeclaration> declarations)
        {
            var existingSubmissionIds = await _dasLevyRepository.GetEmployerDeclarationSubmissionIds(employerLevyData.EmpRef);
            var existingSubmissionIdsLookup = new HashSet<string>(existingSubmissionIds.Select( x => x.ToString()));

            //NOTE: The submissionId in our database is the same as the declaration ID from HMRC (DasDeclaration)
            declarations = declarations.Where(x => !existingSubmissionIdsLookup.Contains(x.Id)).ToArray();

            declarations = declarations.Where(x => !DoesSubmissionPreDateTheLevy(x)).ToArray();

            return declarations.Where(x => !IsSubmissionForFuturePeriod(x)).ToArray();
        }

        private bool DoesSubmissionPreDateTheLevy(DasDeclaration dasDeclaration)
        {
            return _hmrcDateService.DoesSubmissionPreDateLevy(dasDeclaration.PayrollYear);
        }

        private bool IsSubmissionForFuturePeriod(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionForFuturePeriod(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, DateTime.UtcNow);
        }

        private bool IsEndOfYearAdjustment(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionEndOfYearAdjustment(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, dasDeclaration.SubmissionDate);
        }

        private async Task UpdateEndOfYearAdjustment(EmployerLevyData employerLevyData, DasDeclaration dasDeclaration)
        {
            var adjustmentDeclaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(employerLevyData.EmpRef, dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value);
            dasDeclaration.EndOfYearAdjustment = true;
            dasDeclaration.EndOfYearAdjustmentAmount = adjustmentDeclaration?.LevyDueYtd - dasDeclaration.LevyDueYtd ?? 0;
        }
    }
}

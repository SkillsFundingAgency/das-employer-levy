using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Levy;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshEmployerLevyDataCommandHandler _refreshEmployerLevyDataCommandHandler;
        private Mock<IValidator<RefreshEmployerLevyDataCommand>> _validator;
        private Mock<IDasLevyRepository> _levyRepository;
        private Mock<IHmrcDateService> _hmrcDateService;
        private const string ExpectedEmpRef = "123456";

        [SetUp]
        public void Arrange()
        {
            _levyRepository = new Mock<IDasLevyRepository>();
            _levyRepository.Setup(x => x.GetLastSubmissionForScheme(ExpectedEmpRef)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 1000m, LevyAllowanceForFullYear = 1200m });
            
            _validator = new Mock<IValidator<RefreshEmployerLevyDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());
            
            _hmrcDateService = new Mock<IHmrcDateService>();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<DateTime>())).Returns(false);

            _refreshEmployerLevyDataCommandHandler = new RefreshEmployerLevyDataCommandHandler(_validator.Object, _levyRepository.Object, _hmrcDateService.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef));

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>()));
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _refreshEmployerLevyDataCommandHandler.Handle(new RefreshEmployerLevyDataCommand()));
        }

        [Test]
        public async Task ThenTheExistingDeclarationIdsAreCollected()
        {
            //Arrange
            var refreshEmployerLevyDataCommand = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert
            _levyRepository.Verify(x => x.GetEmployerDeclarationSubmissionIds(ExpectedEmpRef), Times.Once());
        }

        [Test]
        public async Task ThenTheLevyRepositoryIsUpdatedIfTheDeclarationDoesNotExist()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclarationSubmissionIds(ExpectedEmpRef)).ReturnsAsync(new List<long>{2});

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef));

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.IsAny<IEnumerable<DasDeclaration>>(), ExpectedEmpRef));
        }

        [Test]
        public async Task ThenIfTheSubmissionIsAnEndOfYearAdjustmentTheFlagIsSetOnTheRecord()
        {
            //Arrange
            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>())).Returns(true);
            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef, "16-17", 12)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 20 });
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Any(d => d.EndOfYearAdjustment)), ExpectedEmpRef), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsAnEndOfYearAdjustmentTheAmountIsWorkedOutFromThePreviousTaxYearValue()
        {
            //Arrange
            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>())).Returns(true);
            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef,"16-17",12)).ReturnsAsync(new DasDeclaration {LevyDueYtd = 20});
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.First().EndOfYearAdjustment && c.First().EndOfYearAdjustmentAmount.Equals(10m)), ExpectedEmpRef), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsForATaxMonthInTheFutureItWillNotBeProcessed()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithFutureSubmissions(ExpectedEmpRef, DateTime.UtcNow);
            var declaration = data.EmployerLevyData.Last().Declarations.Declarations.Last();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(declaration.PayrollYear, declaration.PayrollMonth.Value,It.IsAny<DateTime>())).Returns(true);
            
            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Count() == 4),ExpectedEmpRef), Times.Once);
        }
    }
}

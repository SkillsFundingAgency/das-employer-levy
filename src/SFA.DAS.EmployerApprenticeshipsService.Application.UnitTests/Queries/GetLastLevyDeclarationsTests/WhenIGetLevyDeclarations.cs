﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Queries.AccountTransactions.GetLastLevyDeclaration;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.EmployerLevy.Domain.Models.Levy;

namespace SFA.DAS.EmployerLevy.Application.UnitTests.Queries.GetLastLevyDeclarationsTests
{
    public class WhenIGetLevyDeclarations : QueryBaseTest<GetLastLevyDeclarationQueryHandler, GetLastLevyDeclarationQuery, GetLastLevyDeclarationResponse>
    {
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private const string ExpectedEmpref = "45TGB";

        public override GetLastLevyDeclarationQuery Query { get; set; }
        public override GetLastLevyDeclarationQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetLastLevyDeclarationQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();
            
            _dasLevyRepository = new Mock<IDasLevyRepository>();

            Query = new GetLastLevyDeclarationQuery { EmpRef = ExpectedEmpref };

            RequestHandler = new GetLastLevyDeclarationQueryHandler(RequestValidator.Object, _dasLevyRepository.Object);
        }
        
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyRepository.Verify(x => x.GetLastSubmissionForScheme(ExpectedEmpref));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var expectedDate = new DateTime(2016, 01, 29);
            _dasLevyRepository.Setup(x => x.GetLastSubmissionForScheme(ExpectedEmpref)).ReturnsAsync(new DasDeclaration { SubmissionDate = expectedDate });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual.Transaction);
            Assert.AreEqual(expectedDate, actual.Transaction.SubmissionDate);
        }
        
    }
}

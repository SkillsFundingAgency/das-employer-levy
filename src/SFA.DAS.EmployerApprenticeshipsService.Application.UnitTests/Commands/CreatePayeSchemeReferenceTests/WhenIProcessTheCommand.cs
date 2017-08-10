using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;

namespace SFA.DAS.EmployerLevy.Application.UnitTests.Commands.CreatePayeSchemeReferenceTests
{
    public class WhenIProcessTheCommand
    {
        private CreatePayeSchemeCommandHandler _handler;
        private Mock<IValidator<CreatePayeSchemeCommand>> _validator;
        private Mock<IDasLevyRepository> _dasLevyRepository;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<CreatePayeSchemeCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreatePayeSchemeCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});
            _dasLevyRepository = new Mock<IDasLevyRepository>();

            _handler = new CreatePayeSchemeCommandHandler(_validator.Object, _dasLevyRepository.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _handler.Handle(new CreatePayeSchemeCommand());

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<CreatePayeSchemeCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var expectedEmpRef = "123GBH";

            //Act
            await _handler.Handle(new CreatePayeSchemeCommand {EmpRef = expectedEmpRef});

            //Assert
            _dasLevyRepository.Verify(x=>x.UpsertPayeSchemeReference(expectedEmpRef), Times.Once);
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheCommandIsNotValidAndTheRepositoryNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<CreatePayeSchemeCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "",""} }});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new CreatePayeSchemeCommand()));

            //Assert
            _dasLevyRepository.Verify(x => x.UpsertPayeSchemeReference(It.IsAny<string>()), Times.Never);
        }
    }
}

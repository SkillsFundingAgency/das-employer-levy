using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;

namespace SFA.DAS.EmployerLevy.Application.UnitTests.Commands.DeletePayeSchemeTests
{
    public class WhenIProcessTheCommand
    {
        private DeletePayeSchemeCommandHandler _handler;
        private Mock<IValidator<DeletePayeSchemeCommand>> _validator;
        private Mock<IDasLevyRepository> _dasLevyRepository;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<DeletePayeSchemeCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<DeletePayeSchemeCommand>())).Returns(new ValidationResult ());

            _dasLevyRepository = new Mock<IDasLevyRepository>();

            _handler = new DeletePayeSchemeCommandHandler(_validator.Object,_dasLevyRepository.Object);
        }


        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _handler.Handle(new DeletePayeSchemeCommand {EmpRef = "123RFV"});

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<DeletePayeSchemeCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var expectedEmpRef = "123GBH";

            //Act
            await _handler.Handle(new DeletePayeSchemeCommand { EmpRef = expectedEmpRef });

            //Assert
            _dasLevyRepository.Verify(x => x.DeletePayeSchemeReference(expectedEmpRef), Times.Once);
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheCommandIsNotValidAndTheRepositoryNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<DeletePayeSchemeCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new DeletePayeSchemeCommand()));

            //Assert
            _dasLevyRepository.Verify(x => x.DeletePayeSchemeReference(It.IsAny<string>()), Times.Never);
        }
    }
}

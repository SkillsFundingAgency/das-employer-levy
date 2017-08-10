using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference;

namespace SFA.DAS.EmployerLevy.Application.UnitTests.Commands.DeletePayeSchemeTests
{
    public class WhenIValidateTheCommand
    {
        private DeletePayeSchemeValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new DeletePayeSchemeValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new DeletePayeSchemeCommand {EmpRef = "123DFC"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheRequestIsNotValid()
        {
            //Act
            var actual = _validator.Validate(new DeletePayeSchemeCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmpRef","EmpRef has not been supplied"), actual.ValidationDictionary);
        }
    }
}

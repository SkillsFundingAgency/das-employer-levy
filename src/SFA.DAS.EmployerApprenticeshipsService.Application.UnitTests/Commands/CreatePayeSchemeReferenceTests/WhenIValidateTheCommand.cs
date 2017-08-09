using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference;

namespace SFA.DAS.EmployerLevy.Application.UnitTests.Commands.CreatePayeSchemeReferenceTests
{
    public class WhenIValidateTheCommand
    {
        private CreatePayeSchemeReferenceValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreatePayeSchemeReferenceValidator();
        }

        [Test]
        public void ThenIfAllFieldsArePopulatedThenTrueIsReturned()
        {
            //Act
            var actual = _validator.Validate(new CreatePayeSchemeCommand {EmpRef = "1234/RFV"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenIfTheFieldsAreNotPopulatedThenFalseIsReturnedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new CreatePayeSchemeCommand ());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmpRef","EmpRef has not been supplied"), actual.ValidationDictionary);
        }
    }
}

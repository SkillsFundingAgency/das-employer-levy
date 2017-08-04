using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.LevyAccountUpdater.UnitTests
{
    public class WhenIUpdateAccounts
    {
        private Mock<IMessagePublisher> _messagePublisher;
        private AccountUpdater _updater;
        private Mock<ILog> _logger;
        private Mock<IPayeSchemesRepository> _payeSchemesRepository;
        private List<string> _payeSchemes;

        [SetUp]
        public void Init()
        {
            _payeSchemesRepository = new Mock<IPayeSchemesRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _logger = new Mock<ILog>();

            _payeSchemes = new List<string> { "123ABC", "ZZZ999" };
            _payeSchemesRepository.Setup(x => x.GetPayeSchemes()).ReturnsAsync(_payeSchemes);

            _updater = new AccountUpdater(_messagePublisher.Object, _logger.Object, _payeSchemesRepository.Object);
        }

        [Test]
        public async Task ThenAnUpdateMessageShouldBeAddedToTheProcessQueueForEachPayeScheme()
        {
            //Act
            await _updater.RunUpdate();

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Exactly(2));
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.PayeRef.Equals(_payeSchemes.First()))), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.PayeRef.Equals(_payeSchemes.Last()))), Times.Once);
        }

        [Test]
        public async Task ThenIfNoAccountsAreAvailableTheUpdateShouldNotHappen()
        {
            //Assign
            _payeSchemesRepository.Setup(x => x.GetPayeSchemes()).ReturnsAsync(new List<string>());

            //Act
            await _updater.RunUpdate();

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Never);
        }

        [Test]
        public void ThenIfAnErrorOccursTheErrorShouldBeLogged()
        {
            //Assign
            var exception = new Exception("Test exception");
            _payeSchemesRepository.Setup(x => x.GetPayeSchemes()).Throws(exception);
            _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()));

            //Act
            Assert.ThrowsAsync<Exception>(async () => await _updater.RunUpdate());

            //Assert
            _logger.Verify(x => x.Error(exception, It.IsAny<string>()), Times.Once);
        }
    }
}

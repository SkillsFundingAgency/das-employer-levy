using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.UnitTests.Providers.PayeSchemeRemovedTests
{
    public class WhenHandlingTheTask
    {
        private DeleteDeletePayeScheme _deleteDeletePayeScheme;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        private const string ExpectedEmpRef = "123RFVG";

        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _cancellationTokenSource = new CancellationTokenSource();

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<DeletePayeSchemeMessage>()).
                ReturnsAsync(new FileSystemMessage<DeletePayeSchemeMessage>(stubDataFile, stubDataFile,
                new DeletePayeSchemeMessage { EmpRef = ExpectedEmpRef })).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();

            _logger = new Mock<ILog>();

            _deleteDeletePayeScheme = new DeleteDeletePayeScheme(_pollingMessageReceiver.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReadFromTheQueue()
        {
            //Act
            await _deleteDeletePayeScheme.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _pollingMessageReceiver.Verify(x => x.ReceiveAsAsync<DeletePayeSchemeMessage>(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsCalledWhenTheMessageIsNotEmpty()
        {
            //Act
            await _deleteDeletePayeScheme.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<DeletePayeSchemeCommand>(c => c.EmpRef.Equals(ExpectedEmpRef))), Times.Once());
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var mockFileMessage = new Mock<Message<DeletePayeSchemeMessage>>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<DeletePayeSchemeMessage>())
                                   .ReturnsAsync(mockFileMessage.Object)
                                   .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _deleteDeletePayeScheme.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<DeletePayeSchemeCommand>()), Times.Never());
            mockFileMessage.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task ThenTheLoggerIsCalledIfAnExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<DeletePayeSchemeCommand>())).ThrowsAsync(new Exception());

            //Act
            await _deleteDeletePayeScheme.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _logger.Verify(x => x.Fatal(It.IsAny<Exception>(), It.IsAny<string>()));
        }
    }
}

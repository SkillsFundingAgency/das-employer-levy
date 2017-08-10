using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.UnitTests.Providers.PayeSchemeAddedTests
{
    public class WhenHandlingTheTask
    {
        private PayeSchemeAdded _payeSchemeAdded;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        private const string ExpectedEmpRef = "123RFVG";

        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _cancellationTokenSource = new CancellationTokenSource();

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<AddPayeSchemeMessage>()).
                ReturnsAsync(new FileSystemMessage<AddPayeSchemeMessage>(stubDataFile, stubDataFile,
                new AddPayeSchemeMessage { EmpRef = ExpectedEmpRef })).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();

            _logger = new Mock<ILogger>();

            _payeSchemeAdded = new PayeSchemeAdded(_pollingMessageReceiver.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheMessageIsReadFromTheQueue()
        {
            //Act
            await _payeSchemeAdded.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _pollingMessageReceiver.Verify(x => x.ReceiveAsAsync<AddPayeSchemeMessage>(), Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsCalledWhenTheMessageIsNotEmpty()
        {
            //Act
            await _payeSchemeAdded.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreatePayeSchemeCommand>(c => c.EmpRef.Equals(ExpectedEmpRef))), Times.Once());
        }

        [Test]
        public async Task ThenTheCommandIsNotCalledIfTheMessageIsEmpty()
        {
            //Arrange
            var mockFileMessage = new Mock<Message<AddPayeSchemeMessage>>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<AddPayeSchemeMessage>())
                                   .ReturnsAsync(mockFileMessage.Object)
                                   .Callback(() => { _cancellationTokenSource.Cancel(); });

            //Act
            await _payeSchemeAdded.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreatePayeSchemeCommand>()), Times.Never());
            mockFileMessage.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Test]
        public async Task ThenTheLoggerIsCalledIfAnExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreatePayeSchemeCommand>())).ThrowsAsync(new Exception());

            //Act
            await _payeSchemeAdded.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _logger.Verify(x=>x.Fatal(It.IsAny<Exception>(), It.IsAny<string>()));
        }
    }
}

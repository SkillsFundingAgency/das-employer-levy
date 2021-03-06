﻿using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Application.Commands.CreateEnglishFractionCalculationDate;
using SFA.DAS.EmployerLevy.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerLevy.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.EmployerLevy.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.UnitTests.Providers.LevyDeclarationTests
{
    public class WhenHandlingTheTask
    {
        private const string ExpectedPayeRef = "YHG/123LJH";
        private LevyDeclaration _levyDeclaration;
        private Mock<IPollingMessageReceiver> _pollingMessageReceiver;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        
        [SetUp]
        public void Arrange()
        {
            var stubDataFile = new FileInfo(@"C:\SomeFile.txt");

            _cancellationTokenSource = new CancellationTokenSource();

            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "both";

            _pollingMessageReceiver = new Mock<IPollingMessageReceiver>();
            _pollingMessageReceiver.Setup(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>()).
                ReturnsAsync(new FileSystemMessage<EmployerRefreshLevyQueueMessage>(stubDataFile, stubDataFile,
                new EmployerRefreshLevyQueueMessage { PayeRef = ExpectedPayeRef})).Callback(() => { _cancellationTokenSource.Cancel(); });

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
                .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
                {
                    UpdateRequired = false
                });
            _mediator.Setup(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>())).ReturnsAsync(Unit.Value);

            _logger = new Mock<ILog>();

            _levyDeclaration = new LevyDeclaration(_pollingMessageReceiver.Object, _mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenWhenHmrcHaveUpdatedTheirEnglishFractionCalculationsIShouldUpdateTheLevyCalculations()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Once);
        }
        
        [Test]
        public async Task ThenTheLastCalculationDateForEnglishFractionIsUpdatedIfItHasChanged()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<CreateEnglishFractionCalculationDateCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsNoneNoSchemesAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "none";

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshEmployerLevyDataCommand>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsFractionsThenOnlyFractionsAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "fractions";
            var expectedAccessToken = "myaccesstoken";
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));


            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshEmployerLevyDataCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Never);

        }


        [Test]
        public async Task ThenWhenTheDeclarationsEnabledConfigValueIsDeclarationsThenOnlyDeclarationsAreProcessed()
        {
            //Arrange
            ConfigurationManager.AppSettings["DeclarationsEnabled"] = "declarations";
            _mediator.Setup(x => x.SendAsync(It.Is<GetHMRCLevyDeclarationQuery>(c => c.EmpRef.Equals(ExpectedPayeRef)))).ReturnsAsync(GetHMRCLevyDeclarationResponseObjectMother.Create(ExpectedPayeRef));
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionUpdateRequiredRequest>()))
               .ReturnsAsync(new GetEnglishFractionUpdateRequiredResponse
               {
                   UpdateRequired = true
               });

            //Act
            await _levyDeclaration.RunAsync(_cancellationTokenSource.Token);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<UpdateEnglishFractionsCommand>()), Times.Never);
            _mediator.Verify(x => x.SendAsync(It.IsAny<RefreshEmployerLevyDataCommand>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetHMRCLevyDeclarationQuery>()), Times.Once);
        }
    }
}

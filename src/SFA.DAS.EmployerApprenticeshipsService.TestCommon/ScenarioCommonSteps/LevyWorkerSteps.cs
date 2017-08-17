using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using Moq;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.EmployerLevy.Domain.Interfaces;
using SFA.DAS.EmployerLevy.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.EmployerLevy.TestCommon.DependencyResolution;
using SFA.DAS.EmployerLevy.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;
using StructureMap;

namespace SFA.DAS.EmployerLevy.TestCommon.ScenarioCommonSteps
{
    public class LevyWorkerSteps : IDisposable
    {
        [QueueName("employer_levy")]
        public string get_employer_levy { get; set; }

        private readonly IContainer _container;
        private readonly Mock<IPollingMessageReceiver> _messageReceiver;
        private readonly Mock<IHmrcService> _hmrcService;

        public LevyWorkerSteps()
        {
            //Used to set is processing of declarations should occur
            WebConfigurationManager.AppSettings["DeclarationsEnabled"] = "both";

            _messageReceiver = new Mock<IPollingMessageReceiver>();
            _hmrcService = new Mock<IHmrcService>();

            _container = IoC.CreateLevyWorkerContainer(new Mock<IMessagePublisher>().Object, _messageReceiver.Object, _hmrcService.Object);
        }

        public void RunWorker(IEnumerable<GetHMRCLevyDeclarationResponse> hmrcLevyResponses)
        {
            var levyDeclaration = _container.GetInstance<IMessageProcessor>();
            var levyDeclarationResponses = hmrcLevyResponses as GetHMRCLevyDeclarationResponse[] ?? hmrcLevyResponses.ToArray();

            var cancellationTokenSource = new CancellationTokenSource();
            
            var payeSchemes = levyDeclarationResponses.Select(x => x.Empref).Distinct();

            SetupRefreshLevyMockMessageQueue(payeSchemes, cancellationTokenSource);

            foreach (var declarationResponse in levyDeclarationResponses)
            {
                _hmrcService.Setup(x => x.GetLevyDeclarations(declarationResponse.Empref, It.IsAny<DateTime?>()))
                    .ReturnsAsync(declarationResponse.LevyDeclarations);
            }

            levyDeclaration.RunAsync(cancellationTokenSource.Token).Wait(5000);
        }

        private void SetupRefreshLevyMockMessageQueue(IEnumerable<string> payeSchemes, CancellationTokenSource cancellationTokenSource)
        {
            var setupSequence = _messageReceiver.SetupSequence(x => x.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>());

            foreach (var scheme in payeSchemes)
            {
                var queueMessage = new EmployerRefreshLevyQueueMessage
                {
                    PayeRef = scheme
                };

                var mockMessage = MessageObjectMother.Create(queueMessage, cancellationTokenSource.Cancel, null);

                setupSequence = setupSequence.ReturnsAsync(mockMessage);
            }
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}

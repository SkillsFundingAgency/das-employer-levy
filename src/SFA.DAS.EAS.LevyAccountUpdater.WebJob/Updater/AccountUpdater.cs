using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater
{
    public class AccountUpdater : IAccountUpdater
    {
        private const string ServiceName = "SFA.DAS.EAS.LevyAccountUpdater";
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;
        private readonly IPayeSchemesRepository _payeSchemesRepository;

        [QueueName]
        public string get_employer_levy { get; set; }

        public AccountUpdater(IMessagePublisher messagePublisher, ILog logger, IPayeSchemesRepository payeSchemesRepository)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
            _payeSchemesRepository = payeSchemesRepository;
        }

        public async Task RunUpdate()
        {
            _logger.Info($"{ServiceName}: Running update schedule");

            try
            {
                var timer = Stopwatch.StartNew();

                var tasks = new List<Task>();

                var schemes = await _payeSchemesRepository.GetPayeSchemes();

                foreach (var scheme in schemes)
                {
                    _logger.Trace($"{ServiceName}: Creating update levy account message for scheme {scheme}");
                    tasks.Add(_messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage { PayeRef = scheme }));
                }

                await Task.WhenAll(tasks);

                _logger.Info($"{ServiceName}: update schedule completed in {timer.Elapsed:g} (hh:mm:ss:ms)");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error updating levy accounts");
                throw;
            }
        }
    }
}

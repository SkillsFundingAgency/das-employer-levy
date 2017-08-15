using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Application.Messages;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Attributes;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference
{
    public class CreatePayeSchemeCommandHandler : AsyncRequestHandler<CreatePayeSchemeCommand>
    {
        [QueueName("employer_levy")]
        public string get_employer_levy { get; set; }

        private readonly IValidator<CreatePayeSchemeCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMessagePublisher _messagePublisher;

        public CreatePayeSchemeCommandHandler(IValidator<CreatePayeSchemeCommand> validator, IDasLevyRepository dasLevyRepository, IMessagePublisher messagePublisher)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(CreatePayeSchemeCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _dasLevyRepository.UpsertPayeSchemeReference(message.EmpRef);

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage {PayeRef = message.EmpRef});
        }
    }
}
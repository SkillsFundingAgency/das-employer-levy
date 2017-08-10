using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;

namespace SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference
{
    public class CreatePayeSchemeCommandHandler : AsyncRequestHandler<CreatePayeSchemeCommand>
    {
        private readonly IValidator<CreatePayeSchemeCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public CreatePayeSchemeCommandHandler(IValidator<CreatePayeSchemeCommand> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        protected override async Task HandleCore(CreatePayeSchemeCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _dasLevyRepository.UpsertPayeSchemeReference(message.EmpRef);
        }
    }
}
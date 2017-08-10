using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Application.Validation;
using SFA.DAS.EmployerLevy.Domain.Data.Repositories;

namespace SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference
{
    public class DeletePayeSchemeCommandHandler : AsyncRequestHandler<DeletePayeSchemeCommand>
    {
        private readonly IValidator<DeletePayeSchemeCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public DeletePayeSchemeCommandHandler(IValidator<DeletePayeSchemeCommand> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            
        }

        protected override async Task HandleCore(DeletePayeSchemeCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _dasLevyRepository.DeletePayeSchemeReference(message.EmpRef);
        }
    }
}
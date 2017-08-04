using MediatR;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerLevy.Application.Commands.PublishGenericEvent
{
    public class PublishGenericEventCommand : IAsyncRequest<PublishGenericEventCommandResponse>
    {
        public GenericEvent Event { get; set; }
    }
}

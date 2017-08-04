using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Queries.GetGatewayInformation
{
    public class GetGatewayInformationQuery : IAsyncRequest<GetGatewayInformationResponse>
    {
        public string ReturnUrl { get; set; }
    }
}

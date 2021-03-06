﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerLevy.Domain.Interfaces;

namespace SFA.DAS.EmployerLevy.Application.Queries.GetGatewayInformation
{
    public class GetGatewayInformationHandler : IAsyncRequestHandler<GetGatewayInformationQuery, GetGatewayInformationResponse>
    {
        private readonly IHmrcService _hmrcService;

        public GetGatewayInformationHandler(IHmrcService hmrcService)
        {
            _hmrcService = hmrcService;
        }


        public async Task<GetGatewayInformationResponse> Handle(GetGatewayInformationQuery message)
        {
            var returnUrl = _hmrcService.GenerateAuthRedirectUrl(message.ReturnUrl);

            return new GetGatewayInformationResponse {Url = returnUrl};
        }
    }
}
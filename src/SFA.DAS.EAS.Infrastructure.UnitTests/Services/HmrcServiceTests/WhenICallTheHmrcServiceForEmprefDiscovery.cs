﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerLevy.Domain.Configuration;
using SFA.DAS.EmployerLevy.Domain.Http;
using SFA.DAS.EmployerLevy.Domain.Models.HmrcEmployer;
using SFA.DAS.EmployerLevy.Infrastructure.Services;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;

namespace SFA.DAS.EmployerLevy.Infrastructure.UnitTests.Services.HmrcServiceTests
{
    public class WhenICallTheHmrcServiceForEmprefDiscovery
    {
        private const string ExpectedEmpref = "123/AB12345";
        private string ExpectedBaseUrl = "http://hmrcbase.gov.uk/auth/";
        private string ExpectedClientId = "654321";
        private string ExpectedScope = "emp_ref";
        private string ExpectedClientSecret = "my_secret";
        private const string ExpectedAuthToken = "789654321AGFVD";
        
        private HmrcService _hmrcService;
        private EmployerLevyConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapper;
        private Mock<ITokenServiceApiClient> _tokenService;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerLevyConfiguration
            {
                Hmrc = new HmrcConfiguration
                {
                    BaseUrl = ExpectedBaseUrl,
                    ClientId = ExpectedClientId,
                    Scope = ExpectedScope,
                    ClientSecret = ExpectedClientSecret
                }
            };
            
            _httpClientWrapper = new Mock<IHttpClientWrapper>();
            _httpClientWrapper.Setup(x => x.Get<EmprefDiscovery>(It.IsAny<string>(), "apprenticeship-levy/")).ReturnsAsync(new EmprefDiscovery {Emprefs = new List<string> {ExpectedEmpref} });

            _tokenService = new Mock<ITokenServiceApiClient>();
            _tokenService.Setup(x => x.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken { AccessCode = ExpectedAuthToken });

            _hmrcService = new HmrcService(_configuration, _httpClientWrapper.Object, _tokenService.Object, new NoopExecutionPolicy(),null,null);
        }

        [Test]
        public async Task ThenTheCorrectUrlIsUsedToDiscoverTheEmpref()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            _httpClientWrapper.Verify(x => x.Get<EmprefDiscovery>(authToken, "apprenticeship-levy/"), Times.Once);
        }

        [Test]
        public async Task ThenTheEmprefIsReturned()
        {
            //Arrange
            var authToken = "123FGV";

            //Act
            var actual = await _hmrcService.DiscoverEmpref(authToken);

            //Assert
            Assert.AreEqual(ExpectedEmpref, actual);
        }
    }
}

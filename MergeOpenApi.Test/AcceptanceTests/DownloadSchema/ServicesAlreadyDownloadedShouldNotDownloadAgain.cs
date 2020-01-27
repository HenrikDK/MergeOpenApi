using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Flurl.Http.Testing;
using Lamar;
using MergeOpenApi.Merge;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;
using MergeOpenApi.Model.Queries;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace MergeOpenApi.Test.AcceptanceTests.DownloadSchema
{
    public class ServicesAlreadyDownloadedShouldNotDownloadAgain : AcceptanceTest
    {
        private ServiceDefinition _service;
        private HttpTest _httpClient;

        public ServicesAlreadyDownloadedShouldNotDownloadAgain()
        {
            _registry.AddSingleton(Substitute.For<IUpdateServiceJson>());
            _registry.AddSingleton(Substitute.For<IUpdateServiceStatus>());
            _registry.AddSingleton(Substitute.For<IMergeOpenApiSchemas>());
            
            var getConfiguration = Substitute.For<IGetConfigurationCached>();
            var configuration = new Configuration{UrlFilter = "/v1/", JsonEndpoint = "/swagger.json"};
            getConfiguration.Execute().Returns(configuration);
            _registry.AddSingleton(getConfiguration);
        }
        
        public void GivenAnExistingServiceThatHasBeenFetched()
        {
            var getDeploymentCount = Substitute.For<IGetDeploymentCount>();
            getDeploymentCount.Execute().Returns(1);
            _registry.AddSingleton(getDeploymentCount);
            
            _service = new ServiceDefinition
            {
                Id = 1,
                Status = ServiceStatus.Fetched,
                JsonData = "",
                Retry = 20,
                ServiceUrls = "http://localhost:13000"
            };
            var getActiveServices = Substitute.For<IGetActiveServices>();
            getActiveServices.Execute().Returns(new List<ServiceDefinition> {_service});
            _registry.AddSingleton(getActiveServices);
        }

        public void AndGivenTheServiceIsUnavailable()
        {
            _httpClient = new HttpTest();
            _httpClient.SimulateTimeout();
        }

        public void WhenTheSystemIsRunning()
        {
            _container = new Container(_registry);
            var host = _container.GetInstance<ServiceHost>();
            host.StartAsync(_tokenSource.Token).Wait();
            Thread.Sleep(500);
            _tokenSource.Cancel();
            host.StopAsync(_tokenSource.Token);
        }

        public void ThenTheServiceIsNotContacted()
        {
            _httpClient.ShouldNotHaveMadeACall();
        }
    }
}
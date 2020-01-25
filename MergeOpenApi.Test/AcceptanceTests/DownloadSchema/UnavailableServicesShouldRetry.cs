using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using FluentAssertions;
using Flurl.Http.Testing;
using Lamar;
using MergeOpenApi.Merge;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Queries;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace MergeOpenApi.Test.AcceptanceTests.DownloadSchema
{
    public class UnavailableServicesShouldRetry : AcceptanceTest
    {
        private ServiceDefinition _service;
        private HttpTest _httpClient;

        public UnavailableServicesShouldRetry()
        {
            _registry.AddSingleton(Substitute.For<IUpdateServiceJson>());
            _registry.AddSingleton(Substitute.For<IUpdateServiceStatus>());
            _registry.AddSingleton(Substitute.For<IMergeOpenApiSchemas>());
            
            var getConfiguration = Substitute.For<IGetConfigurationCached>();
            var configuration = new Configuration{UrlFilter = "/v1/", JsonEndpoint = "/swagger.json"};
            getConfiguration.Execute().Returns(configuration);
            _registry.AddSingleton(getConfiguration);
        }
        
        public void GivenASingleServiceHasBeenDeployed()
        {
            var getDeploymentCount = Substitute.For<IGetDeploymentCount>();
            getDeploymentCount.Execute().Returns(1);
            _registry.AddSingleton(getDeploymentCount);
            
            _service = new ServiceDefinition
            {
                Id = 1,
                Status = ServiceStatus.Deployed,
                JsonData = "",
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

        public void ThenTheServiceIsContacted()
        {
            _httpClient
                .ShouldHaveCalled(_service.ServiceUrls + "/swagger.json")
                .WithVerb(HttpMethod.Get);
        }
        
        public void AndThenTheServiceDefinitionIsUpdated()
        {
            var update = _container.GetInstance<IUpdateServiceStatus>();
            update.Received().Execute(Arg.Any<IList<ServiceDefinition>>());

            _service.Status.Should().Be(ServiceStatus.Deployed);
            _service.Retry.Should().BeGreaterThan(0);
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Lamar;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;
using MergeOpenApi.Model.Queries;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace MergeOpenApi.Test.AcceptanceTests.MergeSchema
{
    public class NonPublicUrlsShouldNotBeMerged : AcceptanceTest
    {
        private ServiceDefinition _service;
        private string _primaryJson = @"
{
  ""openapi"": ""3.0.1"",
  ""info"": { ""title"": ""primary"", ""version"": ""v1"" },
  ""paths"": {
    ""/v1/existing"": {},
  },
  ""components"": { ""schemas"": { } }
}";
        
        private ServiceDefinition _primaryService;

        public NonPublicUrlsShouldNotBeMerged()
        {
            var insert = Substitute.For<IInsertMergedSchema>();
            insert.Execute(Arg.Do<string>(x => _primaryJson = x), Arg.Any<int>());
            _registry.AddSingleton(insert);
            
            _registry.AddSingleton(Substitute.For<IUpdateServiceStatus>());
            
            var getConfiguration = Substitute.For<IGetConfigurationCached>();
            var configuration = new Configuration{UrlFilter = "/v1/", JsonEndpoint = "/swagger.json"};
            getConfiguration.Execute().Returns(configuration);
            _registry.AddSingleton(getConfiguration);

        }
        
        public void GivenTwoServicesHaveBeenDeployed()
        {
            var getDeploymentCount = Substitute.For<IGetDeploymentCount>();
            getDeploymentCount.Execute().Returns(2);
            _registry.AddSingleton(getDeploymentCount);
            
            _primaryService = new ServiceDefinition
            {
                Id = 1,
                Status = ServiceStatus.Fetched,
                JsonData = _primaryJson
            };

            var json = @"
{
  ""openapi"": ""3.0.1"",
  ""info"": { ""title"": ""test"", ""version"": ""v1"" },
  ""paths"": {
    ""/v1/deploy"": {},
    ""/merged"": {}
  },
  ""components"": { ""schemas"": { ""TestPet"" : { } } }
}";
            
            _service = new ServiceDefinition
            {
                Id = 2,
                Status = ServiceStatus.Fetched,
                JsonData = json
            };

            var getActiveServices = Substitute.For<IGetActiveServices>();
            getActiveServices.Execute().Returns(new List<ServiceDefinition> {_primaryService, _service});
            _registry.AddSingleton(getActiveServices);
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

        public void ThenNonPublicUrlsAreNotMergedIntoPrimarySchema()
        {
            _primaryJson.Contains("/v1/existing").Should().BeTrue();
            _primaryJson.Contains("/merge").Should().BeFalse();
        }
        
        public void AndThenANewSchemaIsSaved()
        {
            var insert = _container.GetInstance<IInsertMergedSchema>();
            insert.Received().Execute(Arg.Any<string>(), Arg.Any<int>());
        }

        public void AndThenTheServiceDefinitionIsUpdated()
        {
            var update = _container.GetInstance<IUpdateServiceStatus>();
            update.Execute(Arg.Any<IList<ServiceDefinition>>());

            _service.Status.Should().Be(ServiceStatus.Done);
        }
    }
}
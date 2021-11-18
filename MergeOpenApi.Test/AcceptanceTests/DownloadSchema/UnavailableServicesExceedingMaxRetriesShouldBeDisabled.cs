using MergeOpenApi.Merge;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;
using MergeOpenApi.Model.Queries;

namespace MergeOpenApi.Test.AcceptanceTests.DownloadSchema;

public class UnavailableServicesExceedingMaxRetriesShouldBeDisabled : AcceptanceTest
{
    private ServiceDefinition _service;
    private HttpTest _httpClient;

    public UnavailableServicesExceedingMaxRetriesShouldBeDisabled()
    {
        _registry.AddSingleton(Substitute.For<IUpdateServiceJson>());
        _registry.AddSingleton(Substitute.For<IUpdateServiceStatus>());
        _registry.AddSingleton(Substitute.For<IMergeOpenApiSchemas>());
            
        var getConfiguration = Substitute.For<IGetConfigurationCached>();
        var configuration = new Model.Configuration{UrlFilter = "/v1/", JsonEndpoint = "/swagger.json"};
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
        
    public void AndThenTheServiceDefinitionIsDisabled()
    {
        var update = _container.GetInstance<IUpdateServiceStatus>();
        update.Received().Execute(Arg.Any<IList<ServiceDefinition>>());

        _service.Status.Should().Be(ServiceStatus.Disabled);
    }
}
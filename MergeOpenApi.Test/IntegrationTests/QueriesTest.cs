using MergeOpenApi.Infrastructure;
using MergeOpenApi.Model.Queries;

namespace MergeOpenApi.Test.IntegrationTests
{
    public class QueriesTest
    {
        private Container _container;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var registry = new WorkerRegistry();
            registry.AddSingleton(_configuration);
            _container = new Container(registry);
        }

        [Test]
        public void Should_save_service_deployment()
        {
            var getDeploymentCount = _container.GetInstance<IGetDeploymentCount>();

            var count =getDeploymentCount.Execute();

            count.Should().BeGreaterThan(0);
        }
        
        [Test]
        public void Should_get_active_services()
        {
            var getActiveServices = _container.GetInstance<IGetActiveServices>();

            var services = getActiveServices.Execute();

            services.Count.Should().BeGreaterThan(0);
        }
        
        [Test]
        public void Should_get_configuration()
        {
            var getConfiguration = _container.GetInstance<IGetConfiguration>();

            var configuration = getConfiguration.Execute();

            configuration.Should().NotBeNull();
        }
        
        [Test]
        public void Should_get_configuration_in_configuration_ui()
        {
            var registry = new Configuration.Ui.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            var getConfiguration = container.GetInstance<Configuration.Ui.Model.Queries.IGetConfiguration>();

            var configuration = getConfiguration.Execute();

            configuration.Should().NotBeNull();
        }
        
        [Test]
        public void Should_get_services_in_configuration_ui()
        {
            var registry = new Configuration.Ui.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            var getServices = container.GetInstance<Configuration.Ui.Model.Queries.IGetServices>();

            var services = getServices.Execute();

            services.Count.Should().BeGreaterThan(0);
        }
        
        [Test]
        public void Should_get_schema_in_ui()
        {
            var registry = new Ui.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            var getSchema = container.GetInstance<Ui.Model.IGetSchema>();

            var schema = getSchema.Execute();

            schema.Should().NotBeNull();
        }
    }
}
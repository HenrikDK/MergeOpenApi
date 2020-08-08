using System.Collections.Generic;
using System.IO;
using Lamar;
using MergeOpenApi.Api.Model;
using MergeOpenApi.Configuration.Ui.Model;
using MergeOpenApi.Configuration.Ui.Model.Commands;
using MergeOpenApi.Infrastructure;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using IUpdateServiceStatus = MergeOpenApi.Model.Commands.IUpdateServiceStatus;

namespace MergeOpenApi.Test.IntegrationTests
{
    public class CommandTests
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
        public void Should_insert_merged_schema()
        {
            var insert = _container.GetInstance<IInsertMergedSchema>();

            insert.Execute("some_json", 1);
        }

        [Test]
        public void Should_update_json()
        {
            var update = _container.GetInstance<IUpdateServiceJson>();

            var service = new ServiceDefinition
            {
                Id = 1,
                Status = ServiceStatus.Fetched,
                JsonData = "some_test_json",
            };
            
            update.Execute(new List<ServiceDefinition>{ service });
        }

        
        [Test]
        public void Should_update_service_status()
        {
            var update = _container.GetInstance<IUpdateServiceStatus>();

            var service = new ServiceDefinition
            {
                Id = 3,
                Status = ServiceStatus.Deployed,
                JsonData = null,
                Retry = 3,
                ServiceUrls = "test_url"
            };
            
            update.Execute(new List<ServiceDefinition>{ service });
        }
        
        [Test]
        public void Should_save_service_deployment_in_api()
        {
            var registry = new MergeOpenApi.Api.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            
            var save = container.GetInstance<ISaveServiceDeployment>();

            save.Execute("some_test-api", "http://localhost:13001");
        }
        
        [Test]
        public void Should_save_configuration_in_ui()
        {
            var registry = new Configuration.Ui.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            var save = container.GetInstance<ISaveConfiguration>();
            
            var configuration = new Configuration.Ui.Model.Configuration
            {
                Title = "tmp",
                Description = "desc",
                JsonEndpoint = "/swagger.json",
                UrlFilter = "/"
            };
            
            save.Execute(configuration);
        }
        
        [Test]
        public void Should_update_service_status_in_ui()
        {
            var registry = new Configuration.Ui.Infrastructure.ApiRegistry();
            registry.AddSingleton(_configuration);
            var container = new Container(registry);
            var update = container.GetInstance<Configuration.Ui.Model.Commands.IUpdateServiceStatus>();

            var service = new Service
            {
                Id = 3,
                Status = Configuration.Ui.Model.Enums.ServiceStatus.Done,
                Retry = 3,
            };
            
            update.Execute(new List<Service>{ service });
        }
    }
}
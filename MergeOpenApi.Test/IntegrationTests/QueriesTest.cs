using FluentAssertions;
using Lamar;
using MergeOpenApi.Infrastructure;
using MergeOpenApi.Model.Queries;
using NUnit.Framework;

namespace MergeOpenApi.Test.IntegrationTests
{
    public class QueriesTest
    {
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container(new ApiRegistry());
        }

        //[Test]
        public void Should_save_service_deployment()
        {
            var getDeploymentCount = _container.GetInstance<IGetDeploymentCount>();

            var count =getDeploymentCount.Execute();

            count.Should().BeGreaterThan(0);
        }
        
        //[Test]
        public void Should_get_active_services()
        {
            var getActiveServices = _container.GetInstance<IGetActiveServices>();

            var services = getActiveServices.Execute();

            services.Count.Should().BeGreaterThan(0);
        }
    }
}
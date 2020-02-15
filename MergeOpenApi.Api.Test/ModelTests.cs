using FluentAssertions;
using Lamar;
using MergeOpenApi.Api.Infrastructure;
using MergeOpenApi.Api.Model;
using NUnit.Framework;

namespace MergeOpenApi.Api.Test
{
    public class ModelTests
    {
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container(new ApiRegistry());
        }

        [Test]
        public void Should_save_service_deployment()
        {
            var save = _container.GetInstance<ISaveServiceDeployment>();

            save.Execute("some_test-api", "http://localhost:13001");
        }
    }
}
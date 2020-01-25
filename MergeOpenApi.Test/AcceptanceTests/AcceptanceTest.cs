using System.Data;
using System.Threading;
using Lamar;
using MergeOpenApi.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MergeOpenApi.Test.AcceptanceTests
{
    public class AcceptanceTest
    {
        protected CancellationTokenSource _tokenSource = new CancellationTokenSource();

        protected ApiRegistry _registry;
        protected Container _container;

        public AcceptanceTest()
        {
            _registry = new ApiRegistry();

            MockDapper();
            MockLogging();
            MockMemoryCache();
        }
        
        private void MockDapper()
        {
            var connectionFactory = Substitute.For<IConnectionFactory>();
            var connection = Substitute.For<IDbConnection>();
            connectionFactory.Get().Returns(connection);
            _registry.AddSingleton(connectionFactory);
        }
        
        private void MockLogging()
        {
            _registry.AddSingleton(Substitute.For<ILogger>());
        }
        
        private void MockMemoryCache()
        {
            _registry.AddSingleton(Substitute.For<IMemoryCache>());
        }
    }
}
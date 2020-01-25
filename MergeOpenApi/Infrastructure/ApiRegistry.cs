using Lamar;
using MergeOpenApi.Model.Cache;

namespace MergeOpenApi.Infrastructure
{
    public class ApiRegistry : ServiceRegistry
    {
        public ApiRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<Program>();
                
                x.WithDefaultConventions();

                x.LookForRegistries();
                
                x.ExcludeType<ApiRegistry>();
            });

            For<IConnectionFactory>().Use<ConnectionFactory>().Singleton();
            For<IGetConfigurationCached>().Use<GetConfigurationCached>().Singleton();
        }
    }
}

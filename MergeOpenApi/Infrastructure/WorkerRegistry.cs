using Lamar;
using MergeOpenApi.Model.Cache;

namespace MergeOpenApi.Infrastructure
{
    public class WorkerRegistry : ServiceRegistry
    {
        public WorkerRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<Program>();
                
                x.WithDefaultConventions();

                x.LookForRegistries();
                
                x.ExcludeType<WorkerRegistry>();
            });

            For<IConnectionFactory>().Use<ConnectionFactory>().Singleton();
            For<IGetConfigurationCached>().Use<GetConfigurationCached>().Singleton();
        }
    }
}

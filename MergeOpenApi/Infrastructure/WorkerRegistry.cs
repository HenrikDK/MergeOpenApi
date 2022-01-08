using MergeOpenApi.Model.Cache;

namespace MergeOpenApi.Infrastructure;

public class WorkerRegistry : ServiceRegistry
{
    public WorkerRegistry()
    {
        For<IConnectionFactory>().Use<ConnectionFactory>().Singleton();
        For<IGetConfigurationCached>().Use<GetConfigurationCached>().Singleton();
    }
}
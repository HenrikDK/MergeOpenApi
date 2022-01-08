namespace MergeOpenApi.Api.Infrastructure;

public class ApiRegistry : ServiceRegistry
{
    public ApiRegistry()
    {
        For<IConnectionFactory>().Use<ConnectionFactory>().Singleton();
    }
}
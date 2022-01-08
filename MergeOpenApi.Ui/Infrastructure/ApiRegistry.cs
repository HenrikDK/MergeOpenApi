namespace MergeOpenApi.Ui.Infrastructure;

public class ApiRegistry : ServiceRegistry
{
    public ApiRegistry()
    {
        For<IConnectionFactory>().Use<ConnectionFactory>().Singleton();
    }
}
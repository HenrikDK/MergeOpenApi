using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui.Model.Queries;

public interface IGetServices
{
    IList<Service> Execute();
}
    
public class GetServices : IGetServices
{
    private readonly IConnectionFactory _connectionFactory;
        
    public GetServices(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IList<Service> Execute()
    {
        var sql = @" /* MergeOpenApi.Api */
select s.Id, s.Name, s.Status, s.retry
from openapi.service s
where s.IsDelete = false
order by s.Name;";

        using var connection = _connectionFactory.Get();
        return connection.Query<Service>(sql).ToList();
    }
}
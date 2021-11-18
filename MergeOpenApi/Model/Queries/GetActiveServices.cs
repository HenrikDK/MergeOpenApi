using MergeOpenApi.Infrastructure;

namespace MergeOpenApi.Model.Queries;

public interface IGetActiveServices
{
    IList<ServiceDefinition> Execute();
}
    
public class GetActiveServices : IGetActiveServices
{
    private readonly IConnectionFactory _connectionFactory;
        
    public GetActiveServices(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IList<ServiceDefinition> Execute()
    {
        var sql = @" /* MergeOpenApi.Api */
select s.id, s.status, s.jsondata, s.retry, s.serviceUrls
from openapi.service s
where s.Status != 4
and s.IsDelete = false;";

        using var connection = _connectionFactory.Get();
        return connection.Query<ServiceDefinition>(sql).ToList();
    }
}
using MergeOpenApi.Infrastructure;

namespace MergeOpenApi.Model.Commands;

public interface IUpdateServiceJson
{
    void Execute(IList<ServiceDefinition> services);
}
    
public class UpdateServiceJson : IUpdateServiceJson
{
    private readonly IConnectionFactory _connectionFactory;
        
    public UpdateServiceJson(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Execute(IList<ServiceDefinition> services)
    {
        var sql = @" /* MergeOpenApi.Api */
update openapi.service set
    Status = @Status,
    JsonData = @JsonData,
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi',
    Retry = 0
where Id = @Id;";

        using var connection = _connectionFactory.Get();
        connection.Execute(sql, services);
    }
}
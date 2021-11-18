using MergeOpenApi.Infrastructure;

namespace MergeOpenApi.Model.Commands;

public interface IInsertMergedSchema
{
    void Execute(string json, int serviceCount);
}
    
public class InsertMergedSchema : IInsertMergedSchema
{
    private readonly IConnectionFactory _connectionFactory;
        
    public InsertMergedSchema(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Execute(string json, int serviceCount)
    {
        var sql = @" /* MergeOpenApi */
insert into openapi.schema (jsondata, services, created, createdby)
values (@json, @serviceCount, current_timestamp, 'MergeOpenApi');";

        using var connection = _connectionFactory.Get();
        connection.Execute(sql, new { json, serviceCount });
    }
}
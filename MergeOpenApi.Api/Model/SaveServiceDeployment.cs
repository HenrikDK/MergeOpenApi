using MergeOpenApi.Api.Infrastructure;

namespace MergeOpenApi.Api.Model;

public interface ISaveServiceDeployment
{
    void Execute(string name, string serviceUrls);
}
    
public class SaveServiceDeployment : ISaveServiceDeployment
{
    private readonly IConnectionFactory _connectionFactory;
        
    public SaveServiceDeployment(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Execute(string name, string serviceUrls)
    {
        var sql = @" /* MergeOpenApi.Api */
INSERT INTO openapi.service (name, serviceurls, status, created, createdby)
VALUES (@serviceName, @serviceurls, 0, current_timestamp, 'MergeOpenApi.Api')
ON CONFLICT (name)
DO
    UPDATE set
    Status = 0,
    ServiceUrls = @serviceurls,
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi.Api';";

        using var connection = _connectionFactory.Get();
        connection.Execute(sql, new { serviceName = name, serviceUrls });
    }
}
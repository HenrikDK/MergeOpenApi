using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui.Model.Commands;

public interface IUpdateServiceStatus
{
    void Execute(IList<Service> services);
}
    
public class UpdateServiceStatus : IUpdateServiceStatus
{
    private readonly IConnectionFactory _connectionFactory;
        
    public UpdateServiceStatus(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Execute(IList<Service> services)
    {
        var sql = @" /* MergeOpenApi.Configuration.Ui */
update openapi.service set
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi.Configuration.Ui',
    Status = @Status
where Id = @Id;";

        using var connection = _connectionFactory.Get();
        connection.Execute(sql, services);
    }
}
using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui.Model.Commands;

public interface ISaveConfiguration
{
    void Execute(Configuration configuration);
}
    
public class SaveConfiguration : ISaveConfiguration
{
    private readonly IConnectionFactory _connectionFactory;
        
    public SaveConfiguration(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Execute(Configuration configuration)
    {
        var sql = @" /* MergeOpenApi.Configuration.Ui */
insert into openapi.configuration (id, title, description, termsurl, contactemail, licensename, licenseurl, securitytype, securitykeyname, urlfilter, jsonendpoint, created, createdby)
values (1, @title, @description, @termsurl, @contactemail, @licensename, @licenseurl, @securitytype, @securitykeyname, @urlfilter, @jsonendpoint, current_timestamp, 'MergeOpenApi.Configuration.Ui')
ON CONFLICT (id)
DO
    UPDATE set
    title = @title, 
    description = @description, 
    termsurl = @termsurl,
    contactemail = @contactemail,
    licensename = @licensename,
    licenseurl = @licenseurl,
    securitytype = @securitytype, 
    securitykeyname = @securitykeyname,
    urlfilter = @urlfilter,
    jsonendpoint = @jsonendpoint,
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi.Configuration.Ui';";

        using var connection = _connectionFactory.Get();
        connection.Execute(sql, configuration);
    }
}
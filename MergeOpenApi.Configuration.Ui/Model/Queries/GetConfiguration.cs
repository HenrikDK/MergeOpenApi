using System.Linq;
using Dapper;
using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui.Model.Queries
{
    public interface IGetConfiguration
    {
        Configuration Execute();
    }
    
    public class GetConfiguration : IGetConfiguration
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public GetConfiguration(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Configuration Execute()
        {
            var sql = @" /* MergeOpenApi.Api */
select title, description, termsurl, contactemail, licensename, licenseurl, securitytype, securitykeyname,
       urlfilter, jsonendpoint, created, createdby
from openapi.configuration
limit 1;";

            using var connection = _connectionFactory.Get();
            return connection.Query<Configuration>(sql).FirstOrDefault();
        }
    }
}

using System.Linq;
using Dapper;
using MergeOpenApi.Api.Infrastructure;

namespace MergeOpenApi.Api.Model
{
    public interface IGetMergedSchema
    {
        string Execute();
    }
    
    public class GetMergedSchema : IGetMergedSchema
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public GetMergedSchema(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public string Execute()
        {
            var sql = @" /* MergeOpenApi.Api */
select s.JsonData
from openapi.schema s
order by s.id desc
limit 1";

            using (var connection = _connectionFactory.Get())
            {
               return connection.Query<string>(sql).FirstOrDefault();
            }
        }
    }
}

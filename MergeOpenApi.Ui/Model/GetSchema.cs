using System.Linq;
using Dapper;
using MergeOpenApi.Ui.Infrastructure;

namespace MergeOpenApi.Ui.Model
{
    public interface IGetSchema
    {
        string Execute();
    }
    
    public class GetSchema : IGetSchema
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public GetSchema(IConnectionFactory connectionFactory)
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

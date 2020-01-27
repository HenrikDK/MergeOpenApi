using System.Linq;
using Dapper;
using MergeOpenApi.Infrastructure;

namespace MergeOpenApi.Model.Queries
{
    public interface IGetDeploymentCount
    {
        int Execute();
    }
    
    public class GetDeploymentCount : IGetDeploymentCount
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public GetDeploymentCount(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public int Execute()
        {
            var sql = @" /* MergeOpenApi.Api */
select count(s.Id) from openapi.service s
where s.Status not in (3,4);";

            using (var connection = _connectionFactory.Get())
            {
               return connection.Query<int>(sql).FirstOrDefault();
            }
        }
    }
}

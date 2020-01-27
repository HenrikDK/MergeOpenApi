using System.Collections.Generic;
using Dapper;
using MergeOpenApi.Infrastructure;

namespace MergeOpenApi.Model.Commands
{
    public interface IUpdateServiceStatus
    {
        void Execute(IList<ServiceDefinition> services);
    }
    
    public class UpdateServiceStatus : IUpdateServiceStatus
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public UpdateServiceStatus(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Execute(IList<ServiceDefinition> services)
        {
            var sql = @" /* MergeOpenApi.Api */
update openapi.service set
    Status = @Status,
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi',
    Retry = @Retry
where Id = @Id;";

            using (var connection = _connectionFactory.Get())
            {
                connection.Execute(sql, services);
            }
        }
    }
}

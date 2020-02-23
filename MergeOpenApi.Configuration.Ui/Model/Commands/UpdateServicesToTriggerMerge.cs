using Dapper;
using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui.Model.Commands
{
    public interface IUpdateServicesToTriggerMerge
    {
        void Execute();
    }
    
    public class UpdateServicesToTriggerMerge : IUpdateServicesToTriggerMerge
    {
        private readonly IConnectionFactory _connectionFactory;
        
        public UpdateServicesToTriggerMerge(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Execute()
        {
            var sql = @" /* MergeOpenApi.Configuration.Ui */
update openapi.service set
    Modified = current_timestamp,
    ModifiedBy = 'MergeOpenApi.Configuration.Ui',
    Status = 1
where Status = 3;";

            using (var connection = _connectionFactory.Get())
            {
                connection.Execute(sql);
            }
        }
    }
}

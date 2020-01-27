using System.Linq;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Enums;
using MergeOpenApi.Model.Queries;

namespace MergeOpenApi.Merge
{
    public interface IProcessDeployedServices
    {
        void Execute();
    }
    
    public class ProcessDeployedServices : IProcessDeployedServices
    {
        private readonly IFetchServiceDefinitions _fetchServiceDefinitions;
        private readonly IMergeOpenApiSchemas _mergeOpenApiSchemas;
        private readonly IGetConfigurationCached _getConfigurationCached;
        private readonly IGetDeploymentCount _getDeploymentCount;
        private readonly IGetActiveServices _getActiveServices;

        public ProcessDeployedServices(IFetchServiceDefinitions fetchServiceDefinitions,
            IMergeOpenApiSchemas mergeOpenApiSchemas,
            IGetConfigurationCached getConfigurationCached,
            IGetDeploymentCount getDeploymentCount, 
            IGetActiveServices getActiveServices)
        {
            _fetchServiceDefinitions = fetchServiceDefinitions;
            _mergeOpenApiSchemas = mergeOpenApiSchemas;
            _getConfigurationCached = getConfigurationCached;
            _getDeploymentCount = getDeploymentCount;
            _getActiveServices = getActiveServices;
        }
        
        public void Execute()
        {
            var configuration = _getConfigurationCached.Execute();
            if (configuration == null)
            {
                return;
            }
            
            var deployed = _getDeploymentCount.Execute();
            if (deployed == 0)
            {
                return;
            }
            
            var services = _getActiveServices.Execute();

            var notFetched = services.Where(x => x.Status == ServiceStatus.Deployed).ToList();
            if (notFetched.Any())
            {
                _fetchServiceDefinitions.Execute(notFetched);
            }
            
            _mergeOpenApiSchemas.Execute(services);
        }
    }
}

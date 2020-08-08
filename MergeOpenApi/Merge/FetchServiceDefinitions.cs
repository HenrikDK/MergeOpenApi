using System;
using System.Collections.Generic;
using System.Linq;
using Flurl;
using Flurl.Http;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;

namespace MergeOpenApi.Merge
{
    public interface IFetchServiceDefinitions
    {
        void Execute(IList<ServiceDefinition> services);
    }
    
    public class FetchServiceDefinitions : IFetchServiceDefinitions
    {
        private readonly IUpdateServiceJson _updateServiceJson;
        private readonly IGetConfigurationCached _getConfigurationCached;
        private readonly IUpdateServiceStatus _updateServiceStatus;

        public FetchServiceDefinitions(IUpdateServiceJson updateServiceJson,
            IGetConfigurationCached getConfigurationCached,
            IUpdateServiceStatus updateServiceStatus)
        {
            _updateServiceJson = updateServiceJson;
            _getConfigurationCached = getConfigurationCached;
            _updateServiceStatus = updateServiceStatus;
        }
        
        public void Execute(IList<ServiceDefinition> services)
        {
            var exceededRetry = services.Where(x => x.Retry >= 10).ToList();
            if (exceededRetry.Any())
            {
                exceededRetry.ForEach(x => x.Status = ServiceStatus.Disabled);
                _updateServiceStatus.Execute(exceededRetry);
            }
            
            var deployed = services.Except(exceededRetry).ToList();
            deployed.ForEach(FetchServiceDefinition);

            var fetched = deployed.Where(x => x.Status == ServiceStatus.Fetched).ToList();
            var notFetched = deployed.Where(x => x.Status != ServiceStatus.Fetched).ToList();

            if (fetched.Any())
            {
                _updateServiceJson.Execute(fetched);
            }

            if (notFetched.Any())
            {
                _updateServiceStatus.Execute(notFetched);
            }
        }

        private void FetchServiceDefinition(ServiceDefinition service)
        {
            var configuration = _getConfigurationCached.Execute();
            var segment = "/swagger.json";
            if (!string.IsNullOrEmpty(configuration.JsonEndpoint) && configuration.JsonEndpoint.Contains(".json"))
            {
                segment = configuration.JsonEndpoint;
            }
            
            var urls = service.ServiceUrls.Split(',');

            foreach (var url in urls)
            {
                try
                {          
                    var json = url
                        .AppendPathSegment(segment)
                        .GetStringAsync().Result;
                
                    if (json.Length > 0)
                    {
                        service.JsonData = json;
                        service.Status = ServiceStatus.Fetched;
                    }
                }
                catch (Exception e){}

                if (service.Status == ServiceStatus.Fetched)
                {
                    return;
                }
            }

            service.JsonData = null;
            service.Retry++;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;
using MergeOpenApi.Model.Commands;
using MergeOpenApi.Model.Enums;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;

namespace MergeOpenApi.Merge
{
    public interface IMergeOpenApiSchemas
    {
        void Execute(IList<ServiceDefinition> services);
    }
    
    public class MergeOpenApiSchemas : IMergeOpenApiSchemas
    {
        private string mainSpec = @"
{
  ""openapi"": ""3.0.1"",
  ""info"": {""title"": """", ""version"": ""v1""},
  ""paths"": {},
  ""components"": {""schemas"": {}}
}";
        private readonly IGetConfigurationCached _getConfigurationCached;
        private readonly IMergeOpenApiSchema _mergeOpenApiSchema;
        private readonly IInsertMergedSchema _insertMergedSchema;
        private readonly IUpdateServiceStatus _updateServiceStatus;

        public MergeOpenApiSchemas(IGetConfigurationCached getConfigurationCached,
            IMergeOpenApiSchema mergeOpenApiSchema,
            IInsertMergedSchema insertMergedSchema, 
            IUpdateServiceStatus updateServiceStatus)
        {
            _getConfigurationCached = getConfigurationCached;
            _mergeOpenApiSchema = mergeOpenApiSchema;
            _insertMergedSchema = insertMergedSchema;
            _updateServiceStatus = updateServiceStatus;
        }
        
        public void Execute(IList<ServiceDefinition> services)
        {
            if (services.Count(x => x.Status == ServiceStatus.Fetched || x.Status == ServiceStatus.Done) == 1)
            {
                services.First().Status = ServiceStatus.Done;
                using (var scope = new TransactionScope())
                {
                    _insertMergedSchema.Execute(services.First().JsonData, 1);
                    _updateServiceStatus.Execute(services);
                
                    scope.Complete();     
                }
                return;
            }
            
            var mainSchema = JObject.Parse(mainSpec);
            
            services.ForEach(x => MergeServiceIntoMainSchema(mainSchema, x));

            UpdateSchemaMasterData(mainSchema);

            using (var scope = new TransactionScope())
            {
                _insertMergedSchema.Execute(mainSchema.ToString(), services.Count(x => x.Status == ServiceStatus.Done));
                
                _updateServiceStatus.Execute(services);
        
                scope.Complete();
            }
        }
        
        private void MergeServiceIntoMainSchema(JObject mainSchema, ServiceDefinition service)
        {
            try
            {
                _mergeOpenApiSchema.Execute(mainSchema, service);

                service.Status = ServiceStatus.Done;
                service.Retry = 0;
            }
            catch (Exception e)
            {
                service.Retry++;
            }
        }
        
        private void UpdateSchemaMasterData(JObject mainSchema)
        {
            var configuration = _getConfigurationCached.Execute();

            mainSchema["info"]["title"] = configuration.Title;
            mainSchema["info"]["description"] = configuration.Description;

            if (!string.IsNullOrEmpty(configuration.TermsUrl))
            {
                mainSchema["info"]["termsOfService"] = configuration.TermsUrl;
            }
            if (!string.IsNullOrEmpty(configuration.ContactEmail))
            {
                mainSchema["info"]["contact"] = new JObject();
                mainSchema["info"]["contact"]["email"] = configuration.ContactEmail;
            }
            
            if (!string.IsNullOrEmpty(configuration.ContactEmail))
            {
                mainSchema["info"]["license"] = new JObject();
                mainSchema["info"]["license"]["name"] = configuration.LicenseName;
                mainSchema["info"]["license"]["url"] = configuration.LicenseUrl;
            }

            if (configuration.SecurityType == SecurityType.BasicAuth)
            {
                mainSchema["components"]["securitySchemes"] = new JObject();
                mainSchema["components"]["securitySchemes"]["basicAuth"] = new JObject();
                mainSchema["components"]["securitySchemes"]["basicAuth"]["type"] = "http";
                mainSchema["components"]["securitySchemes"]["basicAuth"]["scheme"] = "basic";
            }

            if (configuration.SecurityType == SecurityType.ApiKey)
            {
                mainSchema["components"]["securitySchemes"] = new JObject();
                mainSchema["components"]["securitySchemes"]["ApiKeyAuth"] = new JObject();
                mainSchema["components"]["securitySchemes"]["ApiKeyAuth"]["type"] = "apiKey";
                mainSchema["components"]["securitySchemes"]["ApiKeyAuth"]["in"] = "header";
                mainSchema["components"]["securitySchemes"]["ApiKeyAuth"]["name"] = configuration.SecurityKeyName;
            }
        }
    }
}

using MergeOpenApi.Model;
using MergeOpenApi.Model.Cache;

namespace MergeOpenApi.Merge;

public interface IMergeOpenApiSchema
{
    void Execute(JObject mainSchema, ServiceDefinition service);
}
    
public class MergeOpenApiSchema : IMergeOpenApiSchema
{
    private readonly IGetConfigurationCached _getConfigurationCached;

    public MergeOpenApiSchema(IGetConfigurationCached getConfigurationCached)
    {
        _getConfigurationCached = getConfigurationCached;
    }
        
    public void Execute(JObject mainSchema, ServiceDefinition service)
    {
        var configuration = _getConfigurationCached.Execute();
        var json = JObject.Parse(service.JsonData);

        var schema = service.JsonData; 

        var types = json["components"]["schemas"].ToKeyValuePairs();
        foreach (var type in types)
        {
            schema = schema.Replace($"#/components/schemas/{type.Key}", $"#/components/schemas/{service.Id}_{type.Key}");
        }

        var serviceJson = JObject.Parse(schema);
        foreach (var type in types)
        {
            mainSchema["components"]["schemas"][$"{service.Id}_{type.Key}"] = serviceJson["components"]["schemas"][type.Key];
        }

        var paths = json["paths"].ToKeyValuePairs();
        var urlFilter = configuration.UrlFilter ?? "/";
        foreach (var path in paths)
        {
            if (!path.Key.Contains(urlFilter))
            {
                continue;
            }
                
            mainSchema["paths"][path.Key] = serviceJson["paths"][path.Key];
        }
    }
}
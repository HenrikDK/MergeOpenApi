using MergeOpenApi.Model.Queries;
using Microsoft.Extensions.Caching.Memory;

namespace MergeOpenApi.Model.Cache;

public interface IGetConfigurationCached
{
    Configuration Execute();
}
    
public class GetConfigurationCached : IGetConfigurationCached
{
    private readonly IMemoryCache _cache;
    private readonly IGetConfiguration _getConfiguration;

    public GetConfigurationCached(IMemoryCache cache, IGetConfiguration getConfiguration)
    {
        _cache = cache;
        _getConfiguration = getConfiguration;
    }

    public Configuration Execute()
    {
        return _cache.GetOrCreate("configuration", x =>
        {
            x.SlidingExpiration = TimeSpan.FromMinutes(15);
            return _getConfiguration.Execute();
        });
    }
}
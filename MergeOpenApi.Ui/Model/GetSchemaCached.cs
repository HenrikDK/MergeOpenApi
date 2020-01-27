using System;
using Microsoft.Extensions.Caching.Memory;

namespace MergeOpenApi.Ui.Model
{
    public interface IGetSchemaCached
    {
        string Execute();
    }
    
    public class GetSchemaCached : IGetSchemaCached
    {
        private readonly IMemoryCache _cache;
        private readonly IGetSchema _getSchema;

        public GetSchemaCached(IMemoryCache cache, IGetSchema getSchema)
        {
            _cache = cache;
            _getSchema = getSchema;
        }

        public string Execute()
        {
            return _cache.GetOrCreate("json-schema", x =>
            {
                x.SlidingExpiration = TimeSpan.FromMinutes(2);
                return _getSchema.Execute();
            });
        }
    }
}

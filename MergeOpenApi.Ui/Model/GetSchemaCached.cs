using System;
using Flurl;
using Flurl.Http;
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

        public GetSchemaCached(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string Execute()
        {
            return _cache.GetOrCreate("json-schema", x =>
            {
                x.SlidingExpiration = TimeSpan.FromMinutes(2);
                return GetSchemaJson();
            });
        }

        private static string GetSchemaJson()
        {
            var url = @"http://localhost:13000";

            var json = url
                .AppendPathSegment("/merged")
                .GetStringAsync().Result;

            return json;
        }
    }
}

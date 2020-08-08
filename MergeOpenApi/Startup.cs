using Lamar.Microsoft.DependencyInjection;
using MergeOpenApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace MergeOpenApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLamar(new WorkerRegistry());

            services.AddHostedService<ServiceHost>();
            services.AddMemoryCache();
            
            if (Program.Debug)
            {
                services.AddLogging(x =>
                {
                    x.AddDebug();
                    x.AddConsole();
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            });
        }
    }
}

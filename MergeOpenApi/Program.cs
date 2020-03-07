using System;
using System.Diagnostics;
using System.Linq;
using Lamar.Microsoft.DependencyInjection;
using MergeOpenApi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MergeOpenApi
{
    public class Program
    {
        public static bool Debug = false;

        public static void Main(string[] args)
        {
            if (args.Contains("debug") || Debugger.IsAttached || Environment.GetEnvironmentVariable("debug") != null )
            {
                Debug = true;
            }

            var host = new WebHostBuilder()
                .UseLamar(new ApiRegistry())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ServiceHost>();
                    services.AddMemoryCache();

                    if (Debug)
                    {
                        services.AddLogging(x =>
                        {
                            x.AddDebug();
                            x.AddConsole();
                        });
                    }
                })
                .Build();
          
            host.Run();
        }
    }
}

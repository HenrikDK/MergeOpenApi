using System.Diagnostics;
using System.Linq;
using Lamar.Microsoft.DependencyInjection;
using MergeOpenApi.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MergeOpenApi
{
    public class Program
    {
        public static bool Console = false;

        public static void Main(string[] args)
        {
            if (args.Contains("console") || Debugger.IsAttached)
            {
                Console = true;
            }
            
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ServiceHost>();
                })
                .UseLamar(new ApiRegistry())
                .Build();
          
            host.Run();
        }
    }
}

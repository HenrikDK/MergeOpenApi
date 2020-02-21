using System.Diagnostics;
using System.Linq;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MergeOpenApi.Configuration.Ui
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
            
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseLamar()
                .UseStartup<Startup>()
                .UseUrls("http://*:13006/")
                .Build();
            
            host.Run();
        }
    }
}

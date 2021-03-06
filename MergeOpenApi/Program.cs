using System;
using System.Diagnostics;
using System.Linq;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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

            var host = WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .UseLamar()
                .UseStartup<Startup>()
                .UseUrls("http://*:80")
                .Build();

            host.Run();
        }
    }
}

namespace MergeOpenApi.Api;

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
            .UseUrls("http://*:8080")
            .Build();
            
        host.Run();
    }
}
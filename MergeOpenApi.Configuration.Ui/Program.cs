var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

builder.Host.UseLamar((context, registry) =>
{
    registry.Scan(x =>
    {
        x.AssemblyContainingType<Program>();
        x.WithDefaultConventions();
        x.LookForRegistries();
    });
});

builder.WebHost
    .ConfigureKestrel(x => x.ListenAnyIP(8080))
    .ConfigureLogging((context, config) =>
    {
        config.ClearProviders();
        config.AddSerilog();
    });

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddRazorPages(o => o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

var app = builder.Build();

if (args.Contains("debug") || Debugger.IsAttached || Environment.GetEnvironmentVariable("debug") != null )
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseHttpMetrics();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapMetrics();
});

app.Run();

Log.CloseAndFlush();

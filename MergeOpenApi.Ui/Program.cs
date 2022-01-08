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

var app = builder.Build();

if (args.Contains("debug") || Debugger.IsAttached || Environment.GetEnvironmentVariable("debug") != null )
{
    app.UseDeveloperExceptionPage();
}

app.UseSwaggerUI(x =>
{
    x.RoutePrefix = "";
    x.SwaggerEndpoint("/swagger.json", "MergeOpenApi Api v1");
    x.ConfigObject.AdditionalItems["tagsSorter"] = "alpha";
    x.ConfigObject.DefaultModelsExpandDepth = -1;
    x.DocExpansion(DocExpansion.None);
});

app.UseRouting();
app.UseHttpMetrics();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.Run();

Log.CloseAndFlush();
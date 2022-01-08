using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

builder.WebHost
    .ConfigureKestrel(x => x.ListenAnyIP(8080))
    .UseLamar((context, registry) =>
    {
        // register services using Lamar
        registry.Scan(x =>
        {
            x.AssemblyContainingType<Program>();
            x.WithDefaultConventions();
            x.LookForRegistries();
        });
    })
    .ConfigureLogging((context, config) =>
    {
        config.ClearProviders();
        config.AddSerilog();
    });

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.TagActionsBy(p => new List<string> {"MergeOpenApi - Register deployed REST API services"});
    c.SwaggerDoc("swagger", new OpenApiInfo { Title = "MergeOpenApi API", Version = "v1" });
                
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.UseSwagger(x => x.RouteTemplate = "/{documentName}.json");
app.UseSwaggerUI(x =>
{
    x.RoutePrefix = "";
    x.SwaggerEndpoint("/swagger.json", "MergeOpenApi Api v1");
    x.ConfigObject.DefaultModelsExpandDepth = -1;
});

app.Run();

Log.CloseAndFlush();
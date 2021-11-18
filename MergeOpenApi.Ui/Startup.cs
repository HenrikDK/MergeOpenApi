using MergeOpenApi.Ui.Infrastructure;

namespace MergeOpenApi.Ui;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddLamar(new ApiRegistry());
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

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (Program.Debug || env.IsDevelopment())
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
    }
}
using MergeOpenApi.Configuration.Ui.Infrastructure;

namespace MergeOpenApi.Configuration.Ui;

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
        services.AddRazorPages(o => o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));
        services.AddLamar(new ApiRegistry());

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
    }
}
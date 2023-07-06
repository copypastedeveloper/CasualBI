using Casual.BI.LLM;
using Lamar;
using OpenAI.Interfaces;
using OpenAI.Managers;

namespace Casual.BI.API.Startup;

public class Startup
{
    readonly IConfiguration _configuration;
    
    public static IContainer? Container { get; private set; }
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMvc(options => { options.EnableEndpointRouting = false; });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
    {
        if (!env.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }

        Container = (IContainer) app.ApplicationServices;
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors(cfg => cfg.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthentication();

        app.UseMvc();
        app.UseLLM();
    }

    public void ConfigureContainer(ServiceRegistry services)
    {
        services.AddLogging();

        services.AddControllers();

        services.Scan(s =>
        {
            s.AssembliesFromApplicationBaseDirectory(a => !string.IsNullOrWhiteSpace(a.FullName) &&
                                                          a.FullName.StartsWith("casual.bi",
                                                              StringComparison.OrdinalIgnoreCase));
            s.WithDefaultConventions();
            s.LookForRegistries();
            s.With(new SettingsConvention(_configuration));
        });
        
        services.AddTransient<IOpenAIService>(_ => new OpenAIService(new () {ApiKey = _configuration.GetValue<string>("OpenAI:Key")}));
        // SetupCorsPolicies(services);
        //
        // SetupAuthorization(services);
        //
        // SetupJwtOauthClient(services);
    }
}
using System.Reflection;
using Casual.BI.API.Startup;
using Lamar.Microsoft.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args)
    .UseLamar()
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
    .ConfigureAppConfiguration((builder, config) =>
    {
        var env = builder.HostingEnvironment;

        var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        config
            .SetBasePath(currentDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .AddEnvironmentVariables();
    });

builder.Build().Run();
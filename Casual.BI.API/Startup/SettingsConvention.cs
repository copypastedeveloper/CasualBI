using JasperFx.Core.TypeScanning;
using Lamar;
using Lamar.Scanning.Conventions;

namespace Casual.BI.API.Startup;

public class SettingsConvention : IRegistrationConvention
{
    readonly IConfiguration _configuration;

    public SettingsConvention(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ScanTypes(TypeSet types, ServiceRegistry services)
    {
        var settingTypes = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
            .Where(t =>
            {
                var hasDefaultConstructor = t.GetConstructor(Type.EmptyTypes) != null;
                var isSettingsClass = t.Name.EndsWith("settings", StringComparison.OrdinalIgnoreCase);

                return isSettingsClass && hasDefaultConstructor;
            });

        foreach (var type in settingTypes)
        {
            var config = Activator.CreateInstance(type);
            if (config is null) continue;
            
            _configuration.Bind(type.Name, config);
            services.AddSingleton(type, x => config);
        }
    }
}
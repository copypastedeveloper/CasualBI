using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Casual.BI.LLM;

public static class LLMConfiguration
{
    internal static OpenAISettings? Settings => _openAiSettings ??= _container?.GetInstance<OpenAISettings>();
    static IContainer? _container;
    static OpenAISettings? _openAiSettings;
    
    public static void UseLLM(this IApplicationBuilder builder)
    {
        _container = (IContainer) builder.ApplicationServices;
    }

    public static void UseLLM(this IHost host)
    {
        _container = (IContainer) host.Services;
    }

    public static void Init(OpenAISettings settings)
    {
        _openAiSettings = settings;
    }
}
using Lamar;
using Microsoft.AspNetCore.Builder;

namespace Casual.BI.LLM;

public static class LLMConfiguration
{
    internal static OpenAISettings Settings => _container.GetInstance<OpenAISettings>();
    static IContainer _container;
    
    public static void UseLLM(this IApplicationBuilder builder)
    {
        _container = (IContainer) builder.ApplicationServices;
    }
}
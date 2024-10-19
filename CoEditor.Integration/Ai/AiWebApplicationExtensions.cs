using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Integration.Ai;

public static class AiWebApplicationExtensions
{
    public static void AddAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureOpenAiConfiguration>(configuration.GetSection("AzureOpenAi"));
        services.AddSingleton<IAiConnector, AiConnector>();
    }
}

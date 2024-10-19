using CoEditor.Domain.Outgoing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace CoEditor.Integration.Ai;

public static class AiWebApplicationExtensions
{
    public static void AddAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureOpenAiConfiguration>(configuration.GetSection("AzureOpenAi"));
        services.AddSingleton<IAiConnector, AiConnector>();
    }
}

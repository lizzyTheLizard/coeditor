using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Integration.Ai;

public static class AiWebApplicationExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static void AddAi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAiConnector, AiConnector>();
    }
}

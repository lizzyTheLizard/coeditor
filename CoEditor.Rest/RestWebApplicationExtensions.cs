using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Rest;

public static class RestWebApplicationExtensions
{
    public static void AddRest(this IServiceCollection services)
    {
        services.AddSingleton<LoggingFilter>();
        services.AddControllers();
    }
}

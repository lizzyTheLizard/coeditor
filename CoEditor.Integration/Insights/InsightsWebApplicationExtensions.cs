using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;

namespace CoEditor.Integration.Insights;

public static class InsightsWebApplicationExtensions
{
    public static void AddInsights(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment()) return;
        var connectionString = configuration.GetConnectionString("insights") ??
                               throw new ConfigurationErrorsException("Missing insights connection string");
        services.AddApplicationInsightsTelemetry(c => c.ConnectionString = connectionString);
    }
}

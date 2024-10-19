using CoEditor.Domain.Outgoing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace CoEditor.Integration.Cosmos;

public static class CosmosWebApplicationExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("cosmos")
            ?? throw new ConfigurationErrorsException("Missing cosmos connection string");
        services.AddDbContext<CosmosDbContext>(c => c.UseCosmos(connectionString, "coeditor"));
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();

    }

    public async static Task RecreateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetService<CosmosDbContext>()
            ?? throw new ConfigurationErrorsException("Could not create CosmosDB context");
        await context.Database.EnsureCreatedAsync();
    }
}

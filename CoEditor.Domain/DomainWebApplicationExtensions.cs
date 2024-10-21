using CoEditor.Domain.Incomming;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Domain;

public static class DomainWebApplicationExtensions
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IConversationApi, ConversationService>();
        services.AddScoped<ITemplateApi, TemplateService>();
        services.AddSingleton<PromptMessageFactory>();
    }
}

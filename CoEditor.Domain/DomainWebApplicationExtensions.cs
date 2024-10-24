using CoEditor.Domain.Api;
using CoEditor.Domain.UseCase;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Domain;

public static class DomainWebApplicationExtensions
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IInitializeConversationApi, InitializeConversationUseCase>();
        services.AddScoped<IHandleActionApi, HandleActionUseCase>();
        services.AddScoped<IGetTemplatesApi, GetTemplatesUseCase>();
        services.AddScoped<GetProfileUseCase>();
        services.AddSingleton<PromptMessageFactory>();
    }
}

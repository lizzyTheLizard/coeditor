using CoEditor.Domain.Api;
using CoEditor.Domain.UseCase;
using Microsoft.Extensions.DependencyInjection;

namespace CoEditor.Domain;

public static class DomainWebApplicationExtensions
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IDeleteTemplateApi, DeleteTemplateUseCase>();
        services.AddScoped<IGetProfileApi, GetProfileUseCase>();
        services.AddScoped<IGetTemplatesApi, GetTemplatesUseCase>();
        services.AddScoped<IHandleActionApi, HandleActionUseCase>();
        services.AddScoped<IInitializeConversationApi, InitializeConversationUseCase>();
        services.AddScoped<IUpdateProfileApi, UpdateProfileUseCase>();
        services.AddScoped<IUpdateTemplateApi, UpdateTemplateUseCase>();
        services.AddSingleton<PromptMessageFactory>();
    }
}

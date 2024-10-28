using CoEditor.Client.Rest;
using CoEditor.Client.Services;
using CoEditor.Domain.Api;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace CoEditor.Client;

public static class ClientWebApplicationExtensions
{
    public static void AddServerInteractiveClient(this IServiceCollection services)
    {
        services.AddScoped<UndoService>();
        services.AddScoped<ShortcutService>();
        services.AddScoped<TemplateService>();
        services.AddScoped<ProfileService>();
        services.AddScoped<ConversationService>();
    }

    public static void AddWebAssemblyClient(this IServiceCollection services,
        IWebAssemblyHostEnvironment hostEnvironment)
    {
        services.AddServerInteractiveClient();
        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) });
        services.AddScoped<IGetTemplatesApi, TemplateRestCaller>();
        services.AddScoped<IUpdateTemplateApi, TemplateRestCaller>();
        services.AddScoped<IDeleteTemplateApi, TemplateRestCaller>();
        services.AddScoped<IInitializeConversationApi, ConversationRestCaller>();
        services.AddScoped<IHandleActionApi, ConversationRestCaller>();
        services.AddScoped<IGetProfileApi, ProfileRestCaller>();
        services.AddScoped<IUpdateProfileApi, ProfileRestCaller>();
        // The authentication is done in the backend, the username is stored in the
        // PersistentComponentState and needs to be read by the client
        services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
    }
}

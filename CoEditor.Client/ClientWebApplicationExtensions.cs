using CoEditor.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace CoEditor.Client;

public static class ClientWebApplicationExtensions
{
    public static void AddServerIneractiveClient(this IServiceCollection services)
    {
        services.AddScoped<UndoService>();
        services.AddScoped<ShortcutService>();
        services.AddScoped<IConversationService, ConversationDomainService>();
        services.AddScoped<ITemplateService, TemplateDomainService>();

    }

    public static void AddWebAssemblyClient(this IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
    {
        services.AddScoped<UndoService>();
        services.AddScoped<ShortcutService>();
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) });
        services.AddScoped<IConversationService, ConversationRestService>();
        services.AddScoped<ITemplateService, TemplateRestService>();
        // The authentication is done in the backend, the username is stored in the
        // PersistentComponentState and needs to be read by the client
        services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
    }
}

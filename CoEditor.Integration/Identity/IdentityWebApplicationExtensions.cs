using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Integration.Identity;

public static class IdentityWebApplicationExtensions
{
    public static void AddIServerIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAd");
        services.AddControllersWithViews().AddMicrosoftIdentityUI();
        services.AddAuthorization();
        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
    }
}

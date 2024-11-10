using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace CoEditor.Integration.Identity;

public static class IdentityWebApplicationExtensions
{
    public static void AddServerIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebAppAuthentication(configuration);
        services.AddControllersWithViews().AddMicrosoftIdentityUI();
        services.AddAuthorization();
        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
    }
}

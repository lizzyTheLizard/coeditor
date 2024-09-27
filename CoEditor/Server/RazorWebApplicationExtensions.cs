using CoEditor.Components;
using Microsoft.Identity.Web;

namespace CoEditor.Server;

public static class RazorWebApplicationExtensions
{
    public static WebApplicationBuilder AddRazor(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        return builder;
    }

    public static WebApplication UseRazor(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        return app;
    }

}

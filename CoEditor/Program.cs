using CoEditor.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

//Razor
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//Authentication
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

//Logging
builder.Services.AddHttpLogging(o => { });
//builder.Services.AddApplicationInsightsTelemetry();


var app = builder.Build();
app.UseCustomLogging();
app.UseCustomAuthentication();
app.UseAntiforgery();
app.UseExceptionHandler("/Error", createScopeForErrors: true);
app.UseStaticFiles();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

file static class WebApplicationExtensions
{
    public static void UseCustomLogging(this WebApplication app)
    {
        app.Use(next => (async context =>
        {
            app.Logger.LogInformation("Log Request {} {}", context.Request.Method, context.Request.GetDisplayUrl());
            await next(context);
        }));

        //app.UseHttpLogging();
        app.Use(next => (async context =>
        {
            app.Logger.LogInformation("Log Request {} {}", context.Request.Method, context.Request.GetDisplayUrl());
            await next(context);
        }));
        app.Logger.LogInformation("Adding Logs");

    }

    public static void UseCustomAuthentication(this WebApplication app)
    {
        var authProperties = new AuthenticationProperties { RedirectUri = "/" };
        var endpointBuilder = app.MapGroup("/authentication");
        endpointBuilder.MapPost("/login", () => TypedResults.Challenge(authProperties)).AllowAnonymous();
        endpointBuilder.MapPost("/logout", () => TypedResults.SignOut(authProperties));
    }

}
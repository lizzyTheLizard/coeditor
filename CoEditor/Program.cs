using CoEditor.Components;
using CoEditor.Data;
using CoEditor.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddScoped<UndoService>();
//TODO: Use real DB
builder.Services.AddScoped<UserContext>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.AddInsights();
//Authentication
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();
app.UseErrorHandler();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost });
app.UseStaticFiles();
app.UseAuthenticationEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

file static class WebApplicationExtensions
{
    public static void AddInsights(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment()) return;
        var connectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        builder.Services.AddApplicationInsightsTelemetry(c => c.ConnectionString = connectionString);
    }

    public static void UseErrorHandler(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }
    }

    public static void UseAuthenticationEndpoints(this WebApplication app)
    {
        var authProperties = new AuthenticationProperties { RedirectUri = "/" };
        var endpointBuilder = app.MapGroup("/authentication");
        endpointBuilder.MapPost("/login", () => TypedResults.Challenge(authProperties)).AllowAnonymous();
        endpointBuilder.MapPost("/logout", () => TypedResults.SignOut(authProperties));
    }
}
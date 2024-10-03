using CoEditor.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

//Razor
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//Authentication
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

//Application Insights
builder.Services.AddApplicationInsightsTelemetry(c => {
    c.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]; ;
});


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}
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
    public static void UseAuthenticationEndpoints(this WebApplication app)
    {
        var authProperties = new AuthenticationProperties { RedirectUri = "/" };
        var endpointBuilder = app.MapGroup("/authentication");
        endpointBuilder.MapPost("/login", () => TypedResults.Challenge(authProperties)).AllowAnonymous();
        endpointBuilder.MapPost("/logout", () => TypedResults.SignOut(authProperties));
    }
}
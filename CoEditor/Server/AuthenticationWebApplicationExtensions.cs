using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;

namespace CoEditor.Server;

public static class AuthenticationWebApplicationExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
        return builder;
    }

    public static WebApplication UseAuthentication(this WebApplication app)
    {
        var endpointBuilder = app.MapGroup("/authentication");
        endpointBuilder.MapPost("/login", () => TypedResults.Challenge(GetAuthProperties())).AllowAnonymous();
        endpointBuilder.MapPost("/logout", () => TypedResults.SignOut(GetAuthProperties()));
        return app;
    }


    private static AuthenticationProperties GetAuthProperties()
    {
        return new AuthenticationProperties
        {
            RedirectUri = "/"
        };
    }
}
using CoEditor.Components;
using CoEditor.Components.Pages;
using CoEditor.Data;
using CoEditor.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Configuration;

var app = WebApplication.CreateBuilder(args)
    .AddDatabase()
    .AddInsights()
    .AddRazor()
    .AddSecurity()
    .AddServices()
    .Build();
app.UseErrorHandler();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedHost });
app.UseStaticFiles();
app.UseAuthenticationEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();


// This is only needed when the database has changed...
/*
using (var scope = app.Services.CreateScope())
    using (var context = scope.ServiceProvider.GetService<UserDbContext>())
        await context.Database.EnsureCreatedAsync();
*/

app.Run();

file static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInsights(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment()) return builder;
        var connectionString = builder.Configuration.GetConnectionString("insights");
        builder.Services.AddApplicationInsightsTelemetry(c => c.ConnectionString = connectionString);
        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("cosmos") ?? throw new ConfigurationErrorsException("Missing cosmos connection string");
        builder.Services.AddDbContext<UserDbContext>(c => c.UseCosmos(connectionString, "coeditor"));
        return builder;
    }

    public static WebApplicationBuilder AddRazor(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        return builder;
    }

    public static WebApplicationBuilder AddSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
        return builder;
    }

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<UndoService>();
        builder.Services.AddScoped<AiService>();
        builder.Services.AddScoped<ShortcutService>();
        return builder;
    }
}

file static class WebApplicationExtensions
{ 
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
using Microsoft.AspNetCore.HttpOverrides;

namespace CoEditor.Server;

public static class ReverseProxyWebApplicationExtensions
{
    public static WebApplicationBuilder AddReverseProxyConfig(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
        return builder;
    }

    public static WebApplication UseReverseProxy(this WebApplication app)
    {
        app.UseForwardedHeaders();
        return app;
    }
}

namespace CoEditor.Server;

public static class LogWebApplicationExtensions
{
    public static WebApplicationBuilder AddLog(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpLogging(o => { });
        return builder;
    }

    public static WebApplication UseLog(this WebApplication app)
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHttpLogging();
        app.Logger.LogInformation("Adding Logs");
        return app;
    }
}

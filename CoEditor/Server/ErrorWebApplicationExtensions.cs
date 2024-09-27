namespace CoEditor.Server;

public static class ErrorWebApplicationExtensions
{
    public static WebApplication UseErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        return app;

    }
}

namespace CoEditor.Client.Services;

public class ExceptionService(ILogger<ExceptionService> logger)
{
    private readonly List<Func<Exception, Task>> _handlers = [];

    public RegisterErrorHandlerSubscription RegisterExceptionHandler(Func<Exception, Task> handler)
    {
        logger.ExceptionHandlerRegistered();
        _handlers.Add(handler);
        return new RegisterErrorHandlerSubscription(_handlers, handler, logger);
    }

    public async Task HandleException(Exception exception)
    {
        foreach (var handler in _handlers) await handler(exception);
        logger.ExceptionHandled(exception.Message);
    }
}

public record RegisterErrorHandlerSubscription(
    List<Func<Exception, Task>> Handlers,
    Func<Exception, Task> Handler,
    ILogger Logger) : IDisposable
{
    public void Dispose()
    {
        Logger.ExceptionHandlerUnregistered();
        Handlers.Remove(Handler);
        GC.SuppressFinalize(this);
    }
}

internal static partial class ErrorServiceLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Exception handler registered")]
    public static partial void ExceptionHandlerRegistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Exception handler unregistered")]
    public static partial void ExceptionHandlerUnregistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Show exception {message}")]
    public static partial void ExceptionHandled(this ILogger logger, string message);
}

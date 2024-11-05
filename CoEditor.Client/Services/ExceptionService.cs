namespace CoEditor.Client.Services;

public class ExceptionService(ILogger<ExceptionService> logger)
{
    private readonly List<Func<Exception, Task>> _handlers = [];

    public ExceptionSubscription RegisterExceptionHandler(Func<Exception, Task> handler)
    {
        logger.ExceptionHandlerRegistered();
        _handlers.Add(handler);
        return new ExceptionSubscription(_handlers, handler, logger);
    }

    public async Task HandleException(Exception exception)
    {
        foreach (var handler in _handlers)
        {
            await handler(exception);
        }

        logger.ExceptionHandled(exception.Message);
    }
}

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class ErrorServiceLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Exception handler registered")]
    public static partial void ExceptionHandlerRegistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Exception handler unregistered")]
    public static partial void ExceptionHandlerUnregistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Show exception {message}")]
    public static partial void ExceptionHandled(this ILogger logger, string message);
}

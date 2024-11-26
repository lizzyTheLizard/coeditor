using System.Globalization;

namespace CoEditor.Services;

public class ExceptionService(ILogger<ExceptionService> logger)
{
    private readonly List<Func<string, Task>> _handlers = [];

    public IDisposable RegisterExceptionHandler(Func<string, Task> handler)
    {
        _handlers.Add(handler);
        return new ExceptionSubscription(handler, _handlers);
    }

    public async Task HandleException(Exception exception, int eventId, string messageTemplate, params object[] args)
    {
        var message = string.Format(new CultureInfo("en-US"), messageTemplate, args);
        foreach (var handler in _handlers) await handler(message);
        logger.LogWarning(eventId, exception, "{Message}", message);
    }

    private sealed class ExceptionSubscription(Func<string, Task> handler, List<Func<string, Task>> handlers)
        : IDisposable
    {
        public void Dispose()
        {
            handlers.Remove(handler);
        }
    }
}

namespace CoEditor.Client.Services;

public record ExceptionSubscription(
    List<Func<Exception, Task>> Handlers,
    Func<Exception, Task> Handler,
    ILogger Logger) : IDisposable
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Logger.ExceptionHandlerUnregistered();
        Handlers.Remove(Handler);
    }
}

namespace CoEditor.Client.Services;

public sealed record ExceptionSubscription(
    List<Func<Exception, Task>> Handlers,
    Func<Exception, Task> Handler,
    ILogger Logger) : IDisposable
{
    public void Dispose()
    {
        Logger.ExceptionHandlerUnregistered();
        Handlers.Remove(Handler);
    }
}

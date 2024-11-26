namespace CoEditor.Services;

public class BusyService
{
    private readonly List<Func<Task>> _busyStack = [];
    private readonly List<Func<bool, Task>> _handlers = [];

    public IDisposable RegisterBusyHandler(Func<bool, Task> handler)
    {
        _handlers.Add(handler);
        return new BusySubscription(handler, _handlers);
    }

    public async Task RunWithSpinner(Func<Task> action)
    {
        if (_busyStack.Count == 0)
            foreach (var handler in _handlers)
                await handler(true);
        _busyStack.Add(action);
        try
        {
            await action.Invoke();
        }
        finally
        {
            _busyStack.Remove(action);
            if (_busyStack.Count == 0)
                foreach (var handler in _handlers)
                    await handler(false);
        }
    }

    private sealed class BusySubscription(Func<bool, Task> handler, List<Func<bool, Task>> handlers) : IDisposable
    {
        public void Dispose()
        {
            handlers.Remove(handler);
        }
    }
}

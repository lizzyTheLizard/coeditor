namespace CoEditor.Client.Services;

public record ShortcutSubscription(Dictionary<char, List<Func<Task>>> Callbacks, char Key, Func<Task> Callback, ILogger Logger) : IDisposable
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Logger.ShortcutUnregistered(Key);
        Callbacks[Key].Remove(Callback);
    }
}

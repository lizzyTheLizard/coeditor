namespace CoEditor.Client.Services;

public sealed record ShortcutSubscription(
    Dictionary<char, List<Func<Task>>> Callbacks,
    char Key,
    Func<Task> Callback,
    ILogger Logger) : IDisposable
{
    public void Dispose()
    {
        Logger.ShortcutUnregistered(Key);
        Callbacks[Key].Remove(Callback);
    }
}

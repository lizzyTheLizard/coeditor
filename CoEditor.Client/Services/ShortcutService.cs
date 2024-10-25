using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoEditor.Client.Services;

public class ShortcutService(ILogger<ShortcutService> logger)
{
    private readonly Dictionary<char, List<Func<Task>>> _shortcuts = [];

    [JSInvokable]
    public async Task HandleKeyboardEventAsync(KeyboardEventArgs e)
    {
        var key = e.Key[0];
        if (!e.AltKey)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        var actions = _shortcuts.GetValueOrDefault(key);
        if (actions == null || actions.Count == 0)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        logger.KeyPress(key, e.AltKey, true);
        foreach (var action in actions) await action.Invoke();
    }

    public RegisterShortcutSubscription RegisterShortcut(char key, Func<Task> handler)
    {
        if (!_shortcuts.ContainsKey(key)) _shortcuts[key] = [];
        _shortcuts[key].Add(handler);
        logger.ShortcutRegistered(key);
        return new RegisterShortcutSubscription(_shortcuts, key, handler);
    }
}

public record RegisterShortcutSubscription(Dictionary<char, List<Func<Task>>> Callbacks, char Key, Func<Task> Callback)
    : IDisposable
{
    public void Dispose()
    {
        Callbacks[Key].Remove(Callback);
        GC.SuppressFinalize(this);
    }
}

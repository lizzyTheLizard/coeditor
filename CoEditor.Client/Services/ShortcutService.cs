using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace CoEditor.Client.Services;

public class ShortcutService(ILogger<ShortcutService> logger)
{
    private readonly Dictionary<char, List<Func<Task>>> _shortcuts = [];

    [JSInvokable]
    public async Task HandleKeyboardEventAsync(KeyboardEventArgs e)
    {
        var key = char.ToUpper(e.Key[0], CultureInfo.InvariantCulture );
        var actions = _shortcuts.GetValueOrDefault(key);
        if (!e.AltKey || actions == null || actions.Count == 0)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        logger.KeyPress(key, e.AltKey, true);
        foreach (var action in actions)
            await action.Invoke();
    }

    public RegisterShortcutSubscription RegisterShortcut(char key, Func<Task> handler)
    {
        var registrationKey = char.ToUpper(key, CultureInfo.InvariantCulture);
        if (!_shortcuts.ContainsKey(registrationKey)) _shortcuts[registrationKey] = [];
        _shortcuts[registrationKey].Add(handler);
        logger.ShortcutRegistered(registrationKey);
        return new RegisterShortcutSubscription(_shortcuts, registrationKey, handler, logger);
    }
}

public record RegisterShortcutSubscription(
    Dictionary<char, List<Func<Task>>> Callbacks,
    char Key,
    Func<Task> Callback,
    ILogger Logger)
    : IDisposable
{
    public void Dispose()
    {
        Logger.ShortcutUnregistered(Key);
        Callbacks[Key].Remove(Callback);
        GC.SuppressFinalize(this);
    }
}

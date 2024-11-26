using System.Globalization;
using Microsoft.JSInterop;

namespace CoEditor.Services;

public class ShortcutService
{
    private readonly Dictionary<char, List<Func<Task>>> _shortcuts = [];

    [JSInvokable]
    public async Task<bool> HandleKeyboardEventAsync(char key)
    {
        var upper = char.ToUpper(key, CultureInfo.InvariantCulture);
        var actions = _shortcuts.GetValueOrDefault(upper);
        if (actions == null || actions.Count == 0) return false;
        foreach (var action in actions) await action.Invoke();
        return true;
    }

    public IDisposable RegisterShortcut(char key, Func<Task> handler)
    {
        var registrationKey = char.ToUpper(key, CultureInfo.InvariantCulture);
        if (!_shortcuts.ContainsKey(registrationKey)) _shortcuts[registrationKey] = [];
        _shortcuts[registrationKey].Add(handler);
        return new ShortcutSubscription(_shortcuts, registrationKey, handler);
    }

    private sealed class ShortcutSubscription(
        Dictionary<char, List<Func<Task>>> callbacks,
        char key,
        Func<Task> callback) : IDisposable
    {
        public void Dispose()
        {
            callbacks[key].Remove(callback);
        }
    }
}

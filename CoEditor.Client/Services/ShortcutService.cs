using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoEditor.Client.Services;

public class ShortcutService(ILogger<ShortcutService> logger)
{
    private readonly Dictionary<char, List<Func<Task>>> _shortcuts = [];

    [JSInvokable]
    public async Task HandleKeyboardEventAsync(KeyboardEventArgs e)
    {
        var key = char.ToUpper(e.Key[0], CultureInfo.InvariantCulture);
        var actions = _shortcuts.GetValueOrDefault(key);
        if (!e.AltKey || actions == null || actions.Count == 0)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        logger.KeyPress(key, e.AltKey, true);
        foreach (var action in actions)
        {
            await action.Invoke();
        }
    }

    public ShortcutSubscription RegisterShortcut(char key, Func<Task> handler)
    {
        var registrationKey = char.ToUpper(key, CultureInfo.InvariantCulture);
        if (!_shortcuts.ContainsKey(registrationKey))
        {
            _shortcuts[registrationKey] = [];
        }

        _shortcuts[registrationKey].Add(handler);
        logger.ShortcutRegistered(registrationKey);
        return new ShortcutSubscription(_shortcuts, registrationKey, handler, logger);
    }
}

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class ShortcutServiceLogMessages
{
    [LoggerMessage(LogLevel.Trace, Message = "Shortcut {s} registered")]
    public static partial void ShortcutRegistered(this ILogger logger, char s);

    [LoggerMessage(LogLevel.Trace, Message = "Shortcut {s} unregistered")]
    public static partial void ShortcutUnregistered(this ILogger logger, char s);

    public static void KeyPress(this ILogger logger, char key, bool alt, bool hit)
    {
        var s = (alt ? "Alt+" : string.Empty) + key;
        var hitStr = hit ? "This is a shortcut" : "This is not a shortcut";
        logger.LogTrace("Key {S} pressed. {Hit}", s, hitStr);
    }
}

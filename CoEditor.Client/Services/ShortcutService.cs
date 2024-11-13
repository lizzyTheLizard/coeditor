using System.Globalization;
using Microsoft.JSInterop;

namespace CoEditor.Client.Services;

public class ShortcutService(ILogger<ShortcutService> logger)
{
    private readonly Dictionary<char, List<Func<Task>>> _shortcuts = [];

    [JSInvokable]
    public async Task<bool> HandleKeyboardEventAsync(char key)
    {
        var upper = char.ToUpper(key, CultureInfo.InvariantCulture);
        var actions = _shortcuts.GetValueOrDefault(upper);
        if (actions == null || actions.Count == 0)
        {
            logger.KeyPress(key, false);
            return false;
        }

        logger.KeyPress(key, true);
        foreach (var action in actions) await action.Invoke();
        return true;
    }

    public ShortcutSubscription RegisterShortcut(char key, Func<Task> handler)
    {
        var registrationKey = char.ToUpper(key, CultureInfo.InvariantCulture);
        if (!_shortcuts.ContainsKey(registrationKey)) _shortcuts[registrationKey] = [];
        _shortcuts[registrationKey].Add(handler);
        logger.ShortcutRegistered(registrationKey);
        return new ShortcutSubscription(_shortcuts, registrationKey, handler, logger);
    }
}

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class ShortcutServiceLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Shortcut {s} registered")]
    public static partial void ShortcutRegistered(this ILogger logger, char s);

    [LoggerMessage(LogLevel.Debug, Message = "Shortcut {s} unregistered")]
    public static partial void ShortcutUnregistered(this ILogger logger, char s);

    public static void KeyPress(this ILogger logger, char key, bool hit)
    {
        var hitStr = hit ? "This is a shortcut" : "This is not a shortcut";
        logger.LogDebug("Key {S} pressed. {Hit}", key, hitStr);
    }
}

using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoEditor.Client.Services;

public class ShortcutService(ILogger<ShortcutService> logger)
{
    private readonly Dictionary<char, Func<Task>> _shortcuts = [];

    [JSInvokable]
    public async Task HandleKeyboardEventAsync(KeyboardEventArgs e)
    {
        var key = e.Key[0];
        if (!e.AltKey)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        var action = _shortcuts.GetValueOrDefault(key);
        if (action == null)
        {
            logger.KeyPress(key, e.AltKey, false);
            return;
        }

        logger.KeyPress(key, e.AltKey, true);
        await action.Invoke();
    }

    public void RegisterShortcut(char key, Func<Task> handler)
    {
        _shortcuts[key] = handler;
        logger.ShortcutRegistered(key);
    }
}

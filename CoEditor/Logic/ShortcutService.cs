using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CoEditor.Logic;

public class ShortcutService
{
    private Dictionary<char, Func<Task>> _shortcuts = [];

    [JSInvokable]
    public async Task HandleKeyboardEventAsync(KeyboardEventArgs e)
    {
        if (!e.AltKey) return;
        var action = _shortcuts.GetValueOrDefault(e.Key[0]);
        if (action == null) return;
        await action.Invoke();
        return;
    }

    public void RegisterShortcut(char key, Func<Task> handler)
    {
        _shortcuts[key] = handler;
    }
}

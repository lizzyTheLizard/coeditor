using Bunit;
using CoEditor.Components.Editor;
using CoEditor.Domain.Api;
using CoEditor.Services;

namespace CoEditor.Tests.Client;

public class ActionButtonTests : TestContext
{
    private readonly ShortcutService _shortcutService = new();
    private int _counter;

    public ActionButtonTests()
    {
        Services.AddSingleton(_shortcutService);
    }

    [Fact]
    public async Task NothingGiven_NameIsUndefined()
    {
        var cut = RenderComponent<ActionButton>(parameters => parameters
            .Add(p => p.OnTriggered, () => _counter++)
            .Add(p => p.Disabled, false));
        await AssertButton(cut, 'U', "Undefined");
    }

    [Fact]
    public async Task ActionGiven_NameIsAction()
    {
        var cut = RenderComponent<ActionButton>(parameters => parameters
            .Add(p => p.OnTriggered, () => _counter++)
            .Add(p => p.ActionName, ActionName.Improve)
            .Add(p => p.Disabled, false));
        await AssertButton(cut, 'I', "Improve");
    }

    [Fact]
    public async Task NameGiven_NameIsName()
    {
        var cut = RenderComponent<ActionButton>(parameters => parameters
            .Add(p => p.OnTriggered, () => _counter++)
            .Add(p => p.Name, "Test")
            .Add(p => p.ActionName, ActionName.Improve)
            .Add(p => p.Disabled, false));
        await AssertButton(cut, 'T', "Test");
    }

    [Fact]
    public async Task ShortcutGiven_NameIsShortcut()
    {
        var cut = RenderComponent<ActionButton>(parameters => parameters
            .Add(p => p.OnTriggered, () => _counter++)
            .Add(p => p.Name, "Test")
            .Add(p => p.Shortcut, 'J')
            .Add(p => p.ActionName, ActionName.Improve)
            .Add(p => p.Disabled, false));
        await AssertButton(cut, 'J', "Test");
    }

    [Fact]
    public async Task Disabled_DoNotCall()
    {
        var cut = RenderComponent<ActionButton>(parameters => parameters
            .Add(p => p.OnTriggered, () => _counter++)
            .Add(p => p.Name, "Test")
            .Add(p => p.Shortcut, 'J')
            .Add(p => p.ActionName, ActionName.Improve)
            .Add(p => p.Disabled, true));

        _counter = 0;
        await _shortcutService.HandleKeyboardEventAsync('J');
        Assert.Equal(0, _counter);
        cut.Find("button").Click();
        Assert.Equal(0, _counter);
    }

    private async Task AssertButton(IRenderedComponent<ActionButton> cut, char shortcut, string name)
    {
        Assert.Equal(name, cut.Find(".name").TextContent);
        Assert.Equal($"(Alt + {shortcut})", cut.Find(".shortcut").TextContent);

        _counter = 0;
        await _shortcutService.HandleKeyboardEventAsync(shortcut);
        Assert.Equal(1, _counter);
        cut.Find("button").Click();
        Assert.Equal(2, _counter);
    }
}

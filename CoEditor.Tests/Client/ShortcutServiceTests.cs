using CoEditor.Client.Services;
using Moq;

namespace CoEditor.Tests.Client;

public class ShortcutServiceTests
{
    [Fact]
    public async Task NotRegistered_NotHandled()
    {
        var service = new ShortcutService(Mock.Of<ILogger<ShortcutService>>());
        var handled = await service.HandleKeyboardEventAsync('a');
        Assert.False(handled);
    }

    [Fact]
    public async Task Unregistered_NotHandled()
    {
        var service = new ShortcutService(Mock.Of<ILogger<ShortcutService>>());
        var executed = false;
        var sub = service.RegisterShortcut('a', () =>
        {
            executed = true;
            return Task.CompletedTask;
        });
        sub.Dispose();
        var handled = await service.HandleKeyboardEventAsync('a');
        Assert.False(handled);
        Assert.False(executed);
    }

    [Fact]
    public async Task Registered_Handled()
    {
        var service = new ShortcutService(Mock.Of<ILogger<ShortcutService>>());
        var executed = false;
        var sub = service.RegisterShortcut('a', () =>
        {
            executed = true;
            return Task.CompletedTask;
        });
        var handled = await service.HandleKeyboardEventAsync('a');
        Assert.True(handled);
        Assert.True(executed);
        sub.Dispose();
    }

    [Fact]
    public async Task IgnoreCase()
    {
        var service = new ShortcutService(Mock.Of<ILogger<ShortcutService>>());
        var executed = false;
        var sub = service.RegisterShortcut('a', () =>
        {
            executed = true;
            return Task.CompletedTask;
        });
        var handled = await service.HandleKeyboardEventAsync('A');
        Assert.True(handled);
        Assert.True(executed);
        sub.Dispose();
    }

    [Fact]
    public async Task ExecuteMultiples()
    {
        var service = new ShortcutService(Mock.Of<ILogger<ShortcutService>>());
        var executed1 = false;
        var sub1 = service.RegisterShortcut('a', () =>
        {
            executed1 = true;
            return Task.CompletedTask;
        });
        var executed2 = false;
        var sub2 = service.RegisterShortcut('a', () =>
        {
            executed2 = true;
            return Task.CompletedTask;
        });
        var handled = await service.HandleKeyboardEventAsync('A');
        Assert.True(handled);
        Assert.True(executed1);
        Assert.True(executed2);
        sub1.Dispose();
        sub2.Dispose();
    }
}

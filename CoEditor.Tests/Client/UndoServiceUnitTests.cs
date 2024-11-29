using CoEditor.Services;

namespace CoEditor.Tests.Client;

public class UndoServiceUnitTests
{
    [Fact]
    public void Undo()
    {
        var service = new UndoService();
        service.Reset("Initial Text");
        Assert.False(service.CanUndo);

        service.Register("New Text");
        Assert.True(service.CanUndo);

        var oldtext = service.Undo();
        Assert.False(service.CanUndo);
        Assert.Equal("Initial Text", oldtext);
    }

    [Fact]
    public void Redo()
    {
        var service = new UndoService();
        service.Reset("Initial Text");
        Assert.False(service.CanRedo);

        service.Register("New Text");
        Assert.False(service.CanRedo);

        service.Undo();
        Assert.True(service.CanRedo);

        var oldtext = service.Redo();
        Assert.False(service.CanRedo);
        Assert.Equal("New Text", oldtext);
    }

    [Fact]
    public void Reset()
    {
        var service = new UndoService();
        service.Reset("Initial Text");
        service.Register("New Text");
        service.Register("New Text2");
        service.Undo();
        Assert.True(service.CanRedo);
        Assert.True(service.CanUndo);

        service.Reset("Initial Text 2");
        Assert.False(service.CanUndo);
        Assert.False(service.CanRedo);
    }
}

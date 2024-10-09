namespace CoEditor.Logic;

public class UndoService(ILogger<UndoService> logger)
{
    private readonly Stack<UndoableTextChange> _undo = [];
    private readonly Stack<UndoableTextChange> _redo = [];
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Register(UndoableTextChange textChange)
    {
        logger.LogDebug("Apply text change {change}", textChange);
        _undo.Push(textChange);
        _redo.Clear();
    }

    public void Clean()
    {
        logger.LogDebug("Clear command stacks");
        _undo.Clear();
        _redo.Clear();
    }

    public UndoableTextChange Undo()
    {
        var command = _undo.Pop();
        logger.LogDebug("Undo Command {command}", command);
        _redo.Push(command);
        return command;
    }

    public UndoableTextChange Redo()
    {
        var command = _redo.Pop();
        logger.LogDebug("Redo Command {command}", command);
        _undo.Push(command);
        return command;
    }
}

public record UndoableTextChange(string Name, string TextBefore, string TextAfter) { 
    public UndoableTextChange(string name, TextChange textChange) : this(name, textChange.TextBefore, textChange.TextAfter) { }
}

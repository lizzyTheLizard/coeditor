namespace CoEditor.Client.Services;

public class UndoService(ILogger<UndoService> logger)
{
    private readonly Stack<string> _redo = [];
    private readonly Stack<string> _undo = [];
    private string _current = "";
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Register(string update)
    {
        if (update == _current) return;
        _undo.Push(_current);
        _current = update;
        _redo.Clear();
        logger.TextChangeRegistered();
    }

    public void Reset(string current)
    {
        _current = current;
        _undo.Clear();
        _redo.Clear();
        logger.Cleaned();
    }

    public string Undo()
    {
        var oldText = _undo.Pop();
        _redo.Push(_current);
        _current = oldText;
        logger.Undone();
        return oldText;
    }

    public string Redo()
    {
        var oldText = _redo.Pop();
        _undo.Push(_current);
        _current = oldText;
        logger.Redone();
        return oldText;
    }
}

internal static partial class UndoServiceLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Text-Change registered into Undo chain")]
    public static partial void TextChangeRegistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Undo/Redo chains cleaned")]
    public static partial void Cleaned(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Undone last command")]
    public static partial void Undone(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Redone last command")]
    public static partial void Redone(this ILogger logger);
}

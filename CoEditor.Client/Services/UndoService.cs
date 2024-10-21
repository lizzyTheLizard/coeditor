namespace CoEditor.Client.Services;

public class UndoService
{
    private readonly Stack<string> _redo = [];
    private readonly Stack<string> _undo = [];
    private string _current = "";
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Register(string update)
    {
        _undo.Push(_current);
        _current = update;
        _redo.Clear();
    }

    public void Clean()
    {
        _current = "";
        _undo.Clear();
        _redo.Clear();
    }

    public string Undo()
    {
        var oldText = _undo.Pop();
        _redo.Push(_current);
        _current = oldText;
        return oldText;
    }

    public string Redo()
    {
        var oldText = _redo.Pop();
        _undo.Push(_current);
        _current = oldText;
        return oldText;
    }
}

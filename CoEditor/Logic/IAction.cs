namespace CoEditor.Logic;

public interface IAction
{
    public string Name { get; }
    public Task<UndoableTextChange> ApplyAsync(Context context, string text, Selection? selection);
}

public record Selection(int Start, int End) { }

public record Context(string FilledTemplate, string Profile, string Language) { }

using CoEditor.Data;

namespace CoEditor.Logic;

public abstract class EditorAction
{
    public abstract string GetCommand(CommandInput commandInput);

    public virtual TextChange ApplyResponse(CommandInput commandInput, string response)
    {
        var selection = commandInput.Selection;
        if (selection == null) return new TextChange(commandInput.Text, response);
        var newText = string.Concat(
            commandInput.Text.AsSpan(0, selection.Start),
            response,
            commandInput.Text.AsSpan(selection.End));
        return new TextChange(commandInput.Text, newText);
    }

    public static string GetName(Type type)
    {
        return type.GetCustomAttributes(false)
            .OfType<EditorActionNameAttribute>()
            .Select(a => a.Name)
            .FirstOrDefault(type.Name);
    }

    public static List<char> GetShortcuts(Type type)
    {
        return type.GetCustomAttributes(false)
            .OfType<EditorActionShortCutAttribute>()
            .Select(a => a.Key)
            .ToList();
    }
}


public record CommandInput(Language Language, string Text, string? Context, Selection? Selection) { }

public record Selection(int Start, int End) { }

[AttributeUsage(AttributeTargets.Class)]
public class EditorActionNameAttribute(string name) : Attribute {
    public string Name => name;
}

[AttributeUsage(AttributeTargets.Class)]
public class EditorActionShortCutAttribute(char key) : Attribute
{
    public char Key => key;
}

using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;

namespace CoEditor.Domain;

internal class Reformulate : EditorActionBase
{
    private readonly Dictionary<Language, string> CommandFullText = new()
        {
            { Language.DE, "Formuliere den gesammten Text um"},
            { Language.EN, "Reformulate the whole text"}
        };
    private readonly Dictionary<Language, string> CommandSelection = new()
        {
            { Language.DE, "Formuliere den folgenden Teil um und gib nur die neu formulierte Version zurück: {0}"},
            { Language.EN, "Reformulate the following part and return only the reformulated version: {0}"}
        };

    protected override string GetCommand(Profile profile, EditorActionInput commandInput)
    {
        if (commandInput.Selection == null) return CommandFullText[profile.Language];
        var selection = commandInput.Selection;
        var length = selection.End - selection.Start;
        var selectedText = commandInput.CurrentText.Substring(selection.Start, length);
        return string.Format(CommandSelection[profile.Language], selectedText);
    }
}

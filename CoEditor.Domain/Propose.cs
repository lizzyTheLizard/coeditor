using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;

namespace CoEditor.Domain;

internal class Propose : EditorActionBase
{
    private readonly Dictionary<Language, string> CommandFullText = new()
        {
            { Language.DE, "Mache einen neuen Vorschlag"},
            { Language.EN, "Make a proposal"}
        };

    protected override string GetCommand(Profile profile, EditorActionInput commandInput)
    {
        return CommandFullText[profile.Language];
    }

    public override string ApplyResponse(EditorActionInput commandInput, string response) => response;
}

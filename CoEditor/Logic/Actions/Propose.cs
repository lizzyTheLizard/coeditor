using CoEditor.Data;

namespace CoEditor.Logic.Actions;

[EditorActionShortCut('p')]
public class Propose : EditorAction
{
    private const string FullDe = "Mache einen neuen Vorschlag für diesen Text";
    private const string FullEn = "Make a new proposal for this text";

    public override string GetCommand(CommandInput commandInput)
    {
        return commandInput.Language switch
        {
            Language.DE => FullDe,
            Language.EN => FullEn,
            _ => throw new NotImplementedException("Languague " + commandInput.Language + " is not implemented"),
        };
    }

    public override string ApplyResponse(CommandInput commandInput, string response) => response;
}

using CoEditor.Data;

namespace CoEditor.Logic.Actions;

[EditorActionShortCut('r')]
public class Reformulate : EditorAction
{
    private const string FullDe = "Formuliere den gesammten Text um und gibt nur die neu formulierte Version zurück";
    private const string FullEn = "Reformulate the whole text and return only the reformulated version";
    private const string SelectionDe = "Formuliere den folgenden Teil um und gib nur die neu formulierte Version zurück {0}";
    private const string SelectionEn = "Reformulate the following part and return only the reformulated version: {0}";

    public override string GetCommand(CommandInput commandInput)
    {
        if(commandInput.Selection == null)
        {
            return commandInput.Language switch
            {
                Language.DE => FullDe,
                Language.EN => FullEn,
                _ => throw new NotImplementedException("Languague " + commandInput.Language + " is not implemented"),
            };
        } else
        {
            var selection = commandInput.Selection;
            var length = selection.End - selection.Start;
            return commandInput.Language switch
            {
                Language.DE => string.Format(SelectionDe, commandInput.Text.Substring(selection.Start, length)),
                Language.EN => string.Format(SelectionEn, commandInput.Text.Substring(selection.Start, length)),
                _ => throw new NotImplementedException("Languague " + commandInput.Language + " is not implemented"),
            };
        }
    }
}

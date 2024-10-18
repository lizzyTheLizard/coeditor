using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;

namespace CoEditor.Domain;

internal abstract class EditorActionBase
{
    private readonly Dictionary<Language, string> CurrentTextInto = new()
        {
            { Language.DE, "Bis jezt habe ich folgendes: {2}"},
            { Language.EN, "So far I have the following: {2}"}
        };

    public virtual string GetPrompt(Profile profile, EditorActionInput commandInput)
    {
        var profileText = $"{profile.Text}\n\n";
        var contextText = $"{commandInput.Context}\n\n";
        var currentText = string.IsNullOrEmpty(commandInput.CurrentText)
            ? ""
            : $"{CurrentTextInto[profile.Language]} {commandInput.CurrentText}\n\n";
        var commandText = GetCommand(profile, commandInput);
        return profileText + contextText + currentText + commandText;
    }

    protected abstract string GetCommand(Profile profile, EditorActionInput commandInput);

    public virtual string ApplyResponse(EditorActionInput commandInput, string response)
    {
        var selection = commandInput.Selection;
        if (selection == null) return response;
        var newText = string.Concat(
            commandInput.CurrentText.AsSpan(0, selection.Start),
            response,
            commandInput.CurrentText.AsSpan(selection.End));
        return newText;
    }
}

using CoEditor.Data;

namespace CoEditor.Logic;

public class AiService (UserContext userContext, ILogger<AiService> logger)
{
    private const string PromptDe = "{0}\nIch will dass du mir hilft folgendes zu schreiben: {1}\nBis jezt habe ich folgendes: {2}\n\n{3}";
    private const string PromptEn = "{0}\nI want you to help me write the following: {1}\nSo far I have the following: {2}\n\n{3}";
    private const string PromptDeNoContext = "{0}\nIch will dass du mir hilft einen Text zu schreiben\nBis jezt habe ich folgendes: {1}\n\n{2}";
    private const string PromptEnNoContext = "{0}\nI want you to help me write a text\nSo far I have the following: {1}\n\n{2}";

    public async Task<TextChange> RunActionAsync(EditorAction action, CommandInput commandInput)
    {
        var message = await GetMessageAsync(action, commandInput);
        logger.LogInformation("Send prompt to AI: {}", message);
        //TODO: Implement AI
        var response = "Replacement";
        await Task.Delay(1000);
        return action.ApplyResponse(commandInput, response);
    }


    private async Task<string> GetMessageAsync(EditorAction action, CommandInput commandInput)
    {
        var profile = await userContext.GetProfileAsync(commandInput.Language);
        var command = action.GetCommand(commandInput);
        if (commandInput.Context == null)
        {
            return commandInput.Language switch
            {
                Language.DE => string.Format(PromptDeNoContext, profile.Text, commandInput.Text, command),
                Language.EN => string.Format(PromptEnNoContext, profile.Text, commandInput.Text, command),
                _ => throw new NotImplementedException("Languague " + commandInput.Language + " is not implemented"),
            };

        }
        else
        {
            return commandInput.Language switch
            {
                Language.DE => string.Format(PromptDe, profile.Text, commandInput.Context, commandInput.Text, command),
                Language.EN => string.Format(PromptEn, profile.Text, commandInput.Context, commandInput.Text, command),
                _ => throw new NotImplementedException("Languague " + commandInput.Language + " is not implemented"),
            };
        }
    }
}

public record TextChange(string TextBefore, string TextAfter) { }

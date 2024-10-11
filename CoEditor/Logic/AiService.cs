using CoEditor.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Logic;

public class AiService(AuthenticationStateProvider authenticationStateProvider, UserDbContext userContext, ILogger<AiService> logger)
{
    private readonly Dictionary<Language, string> Prompt = new Dictionary<Language, string>()
        {
            { Language.DE, "{0}\nIch will dass du mir hilft folgendes zu schreiben: {1}\nBis jezt habe ich folgendes: {2}\n\n{3}"},
            { Language.EN, "{0}\nI want you to help me write the following: {1}\nSo far I have the following: {2}\n\n{3}"}
        };
    private readonly Dictionary<Language, string> DefaultProfile = new Dictionary<Language, string>()
        {
            { Language.DE, "Ich bin ein neuer Benutzer"},
            { Language.EN, "I am a new user"}
        };

    public async Task<string> RunActionAsync(EditorAction action, CommandInput commandInput)
    {
        var command = action.GetCommand(commandInput);
        var profile = await GetProfileAsync(commandInput.Language);
        var message = string.Format(Prompt[commandInput.Language], profile, commandInput.Context, commandInput.Text, command);
        logger.LogInformation("Send prompt to AI: {}", message);
        //TODO: Implement AI
        var response = "Replacement";
        await Task.Delay(1000);
        return action.ApplyResponse(commandInput, response);
    }

    private async Task<string> GetProfileAsync(Language language)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = authenticationState.User.Identity ?? throw new Exception("User must be logged in ");
        var userName = identity.Name ?? throw new Exception("User must have a name");
        var profile = await userContext.Profiles
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .FirstOrDefaultAsync();
        return profile?.Text ?? DefaultProfile[language];
    }
}

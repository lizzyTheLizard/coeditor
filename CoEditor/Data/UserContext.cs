using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Data;

public class UserContext(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<Template[]> GetTemplatesAsync(Language language)
    {
        var userName = await GetUserNameAsync();
        return [
            new Template() { Name = "Free", Text = "{context%textarea}", UserName = userName, Language = language },
            new Template() { Name = "Email Collegue", Text = "Write an email to my work collegue {name%text}. I use \"Hi\" as greeting and \"See you\" as farewell. The mail is about {content%textarea}. Use only my surname in the farewell", UserName = userName, Language = language },
            new Template() { Name = "Joke", Text = "Write ajoke with {Number of Sentences%int} Sentences", UserName = userName, Language = language },
        ];
    }

    public async Task<Profile> GetProfileAsync(Language language)
    {
        var userName = await GetUserNameAsync();
        return new Profile() { Text = "My name is Matthias Graf and I live in Bern", UserName = userName, Language = language };
    }

    private async Task<string> GetUserNameAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = authenticationState.User.Identity;
        if (identity == null) throw new Exception("User must be logged in ");
        var username = identity.Name;
        if (string.IsNullOrEmpty(username)) throw new Exception("User must have a name");
        return username;
    }
}

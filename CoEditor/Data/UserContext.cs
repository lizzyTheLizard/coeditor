namespace CoEditor.Data;

public class UserContext
{
    public Template[] GetTemplates(string userName, string language)
    {
        //TODO: Implement DB access
        return [
            new Template() { Name = "Free", Text = "{context%textarea}", UserName = userName, Language = language },
            new Template() { Name = "Email Collegue", Text = "Write an email to my work collegue {name%text}. I use \"Hi\" as greeting and \"See you\" as farewell. The mail is about {content%textarea}. Use only my surname in the farewell", UserName = userName, Language = language },
            new Template() { Name = "Joke", Text = "Write ajoke with {Number of Sentences%int} Sentences", UserName = userName, Language = language },
        ];
    }

    public Profile GetProfile(string userName, string language)
    {
        //TODO: Implement DB access
        return new Profile() { Text = "My name is Matthias Graf and I live in Bern", UserName = userName, Language = language };
    }
}

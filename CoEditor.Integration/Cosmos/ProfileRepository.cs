using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class ProfileRepository(CosmosDbContext _dbContext) : IProfileRepository
{
    private readonly Dictionary<Language, string> DefaultProfileText = new()
    {
        { Language.DE, "Ich bin ein neuer Benutzer" }, { Language.EN, "I am a new user" }
    };

    public async Task<Profile> GetProfileAsync(string userName, Language language)
    {
        var profile = await _dbContext.Profiles
            .Where(t => t.Language == language)
            .Where(t => t.UserName == userName)
            .Select(t => new Profile { Text = t.Text, Language = t.Language })
            .FirstOrDefaultAsync();
        return profile ?? new Profile { Language = language, Text = DefaultProfileText[language] };
    }
}

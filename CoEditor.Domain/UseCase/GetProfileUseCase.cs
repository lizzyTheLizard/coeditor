using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetProfileUseCase(
    IProfileRepository profileRepository,
    ILogger<GetProfileUseCase> logger)
{
    private readonly Dictionary<Language, string> DefaultProfileText = new()
    {
        { Language.De, "Ich bin ein neuer Benutzer" }, { Language.En, "I am a new user" }
    };

    public async Task<Profile> GetProfileAsync(string userName, Language language)
    {
        var userProfile = await profileRepository.GetProfileAsync(userName, language);
        var profile = userProfile ?? new Profile { Language = language, Text = DefaultProfileText[language] };
        logger.ProfileLoaded(profile, userProfile == null, userName, language);
        return profile;
    }
}

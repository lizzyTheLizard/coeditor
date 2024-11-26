using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetProfileUseCase(
    IProfileRepository profileRepository,
    IUserService userService,
    ILogger<GetProfileUseCase> logger)
    : IGetProfileApi
{
    private readonly Dictionary<Language, string> _defaultProfileText = new()
    {
        {
            Language.De,
            "Ich bin ein neuer Benutzer und es sind noch keine Informationen über mich gespeichert. Verwende einfach die Standart-Einstellungen."
        },
        {
            Language.En,
            "I am a new user and there is no information stored about me. Just use default settings."
        }
    };

    public async Task<Profile> GetProfileAsync(Language language)
    {
        var userName = await userService.GetUserNameAsync();
        var userProfile = await profileRepository.FindProfileAsync(userName, language);
        var profile = userProfile ??
                      new Profile { Language = language, UserName = userName, Text = _defaultProfileText[language] };
        logger.ProfileLoaded(profile, userProfile == null);
        return profile;
    }
}

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static class GetProfileLogMessages
{
    public static void ProfileLoaded(this ILogger logger, Profile profile, bool defaultProfile)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;

        var type = defaultProfile ? "default" : "user";
        logger.LogDebug(
            "Loaded {Type} profile of {UserName} in {Language}. Text size is {TextSize}",
            type,
            profile.UserName,
            profile.Language,
            profile.Text.Length);
        if (!logger.IsEnabled(LogLevel.Trace)) return;

        logger.LogTrace("{Profile}", profile);
    }
}

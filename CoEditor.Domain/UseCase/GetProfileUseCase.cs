﻿using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetProfileUseCase(
    IProfileRepository profileRepository,
    ILogger<GetProfileUseCase> logger) : IGetProfileApi
{
    private readonly Dictionary<Language, string> defaultProfileText = new()
    {
        { Language.De, "Ich bin ein neuer Benutzer und es sind noch keine Informationen über mich gespeichert. Verwende einfach die Standart-Einstellungen." },
        { Language.En, "I am a new user and there is no information stored about me. Just use default settings." },
    };

    public async Task<Profile> GetProfileAsync(string userName, Language language)
    {
        var userProfile = await profileRepository.FindProfileAsync(userName, language);
        var profile = userProfile ??
                      new Profile { Language = language, UserName = userName, Text = defaultProfileText[language] };
        logger.ProfileLoaded(profile, userProfile == null, userName, language);
        return profile;
    }
}

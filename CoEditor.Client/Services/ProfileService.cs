using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class ProfileService(
    IGetProfileApi getProfileApi,
    IUpdateProfileApi updateProfileApi,
    AuthenticationStateProvider authenticationStateProvider,
    ExceptionService exceptionService,
    ILogger<ProfileService> logger)
{
    public async Task<string> GetProfileAsync(Language language)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            var profile = await getProfileApi.GetProfileAsync(userName, language);
            logger.ProfileLoaded(profile);
            return profile.Text;
        }
        catch (Exception e)
        {
            logger.ProfileLoadingFailed(e, language);
            await exceptionService.HandleException(e);
            return "";
        }
    }

    public async Task UpdateProfile(Language language, string text)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var userName = authenticationState.User.Identity?.Name ?? "";
        var profile = new Profile { Language = language, Text = text, UserName = userName };
        try
        {
            await updateProfileApi.UpdateProfileAsync(userName, profile);
            logger.ProfileUpdated(profile);
        }
        catch (Exception e)
        {
            logger.ProfileUpdateFailed(e, profile);
            await exceptionService.HandleException(e);
        }
    }
}

internal static partial class ProfileServiceLogMessages
{
    public static void ProfileLoaded(this ILogger logger, Profile profile)
    {
        logger.ProfileLoaded(profile.Language);
        logger.TraceProfile(profile);
    }

    [LoggerMessage(LogLevel.Trace, Message = "{profile}")]
    private static partial void TraceProfile(this ILogger logger, Profile profile);

    [LoggerMessage(LogLevel.Debug, Message = "Profile for language {language} loaded")]
    private static partial void ProfileLoaded(this ILogger logger, Language language);

    [LoggerMessage(LogLevel.Warning, EventId = 2401, Message = "Could not load profile for language {language}")]
    public static partial void ProfileLoadingFailed(this ILogger logger, Exception e, Language language);

    public static void ProfileUpdated(this ILogger logger, Profile profile)
    {
        logger.ProfileUpdated(profile.Language);
        logger.TraceProfile(profile);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2402, Message = "Updated profile for language {language}")]
    private static partial void ProfileUpdated(this ILogger logger, Language language);

    public static void ProfileUpdateFailed(this ILogger logger, Exception e, Profile profile)
    {
        logger.ProfileUpdateFailed(profile.Language);
        logger.TraceProfile(profile);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2403, Message = "Updated profile for language {language} failed")]
    private static partial void ProfileUpdateFailed(this ILogger logger, Language language);
}

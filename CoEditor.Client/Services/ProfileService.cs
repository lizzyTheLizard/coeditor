using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Services;

public class ProfileService(
    IGetProfileApi getProfileApi,
    IUpdateProfileApi updateProfileApi,
    UserService userService,
    ExceptionService exceptionService,
    ILogger<ProfileService> logger)
{
    public async Task<Profile?> GetProfileAsync(Language language)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
            var profile = await getProfileApi.GetProfileAsync(userName, language);
            logger.ProfileLoaded(profile);
            return profile;
        }
        catch (Exception e)
        {
            logger.ProfileLoadingFailed(e, language);
            await exceptionService.HandleException(e);
            return null;
        }
    }

    public async Task UpdateProfile(Profile profile)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
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

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class ProfileServiceLogMessages
{
    public static void ProfileLoaded(this ILogger logger, Profile profile)
    {
        logger.LogDebug("Profile for language {Language} loaded", profile.Language);
        logger.LogTrace("{Profile}", profile);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2401, Message = "Could not load profile for language {language}")]
    public static partial void ProfileLoadingFailed(this ILogger logger, Exception e, Language language);

    public static void ProfileUpdated(this ILogger logger, Profile profile)
    {
        logger.LogInformation(2402, "Updated profile for language {Language}", profile.Language);
        logger.LogTrace("{Profile}", profile);
    }

    public static void ProfileUpdateFailed(this ILogger logger, Exception e, Profile profile)
    {
        logger.LogWarning(2403, e, "Updated profile for language {Language} failed", profile.Language);
        logger.LogTrace("{Profile}", profile);
    }
}

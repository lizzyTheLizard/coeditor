using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class ProfileService(
    IGetProfileApi getProfileApi,
    IUpdateProfileApi updateProfileApi,
    AuthenticationStateProvider authenticationStateProvider,
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
            //TODO: Error Handling: Show error to user!
            logger.ProfileLoadingFailed(e, language);
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
            //TODO: Error Handling: Show error to user!
            logger.ProfileUpdateFailed(e, profile);
        }
    }
}

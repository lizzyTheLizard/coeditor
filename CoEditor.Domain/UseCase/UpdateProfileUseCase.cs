using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class UpdateProfileUseCase(
    IProfileRepository profileRepository,
    ILogger<UpdateProfileUseCase> logger) : IUpdateProfileApi
{
    public async Task<Profile> UpdateProfileAsync(string userName, Profile profile)
    {
        if (profile.UserName != userName)
        {
            throw new ArgumentException("Wrong user name in body");
        }

        var originalProfile = await profileRepository.FindProfileAsync(userName, profile.Language);
        if (originalProfile == null)
        {
            var createResult = await profileRepository.CreateProfileAsync(userName, profile);
            logger.ProfileCreated(profile);
            return createResult;
        }

        var updateResult = await profileRepository.UpdateProfileAsync(userName, profile);
        logger.ProfileUpdated(profile);
        return updateResult;
    }
}

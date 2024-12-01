using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface IProfileRepository
{
    Task<Profile?> FindProfileAsync(string userName, Language language);

    Task<Profile> CreateProfileAsync(Profile profile);

    Task<Profile> UpdateProfileAsync(Profile profile);

    Task DeleteAllProfilesAsync(string userName);
}

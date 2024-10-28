using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface IProfileRepository
{
    Task<Profile?> FindProfileAsync(string userName, Language language);

    Task<Profile> CreateProfileAsync(string userName, Profile profile);

    Task<Profile> UpdateProfileAsync(string userName, Profile profile);
}

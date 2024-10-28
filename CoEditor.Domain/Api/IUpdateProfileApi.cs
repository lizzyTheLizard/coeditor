using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IUpdateProfileApi
{
    Task<Profile> UpdateProfileAsync(string userName, Profile profile);
}

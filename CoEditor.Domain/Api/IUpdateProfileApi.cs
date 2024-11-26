using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IUpdateProfileApi
{
    Task<Profile> UpdateProfileAsync(Profile profile);
}

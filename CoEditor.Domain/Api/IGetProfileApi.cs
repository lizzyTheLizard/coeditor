using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IGetProfileApi
{
    Task<Profile> GetProfileAsync(Language language);
}

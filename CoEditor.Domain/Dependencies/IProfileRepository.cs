using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface IProfileRepository
{
    Task<Profile?> GetProfileAsync(string userName, Language language);
}

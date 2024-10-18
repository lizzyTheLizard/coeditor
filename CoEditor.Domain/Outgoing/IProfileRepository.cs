using CoEditor.Domain.Model;

namespace CoEditor.Domain.Outgoing;

public interface IProfileRepository
{
    Task<Profile> GetProfileAsync(string userName, Language language);
}

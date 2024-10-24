using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IGetTemplatesApi
{
    Task<Template[]> GetTemplatesAsync(string userName, Language language);
}

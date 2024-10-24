using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface ITemplateRepository
{
    Task<Template[]> GetTemplatesAsync(string userName, Language language);
}

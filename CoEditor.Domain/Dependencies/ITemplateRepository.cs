using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface ITemplateRepository
{
    Task<Template[]> GetTemplatesAsync(string userName, Language language);

    Task DeleteTemplateAsync(Guid id);

    Task<Template?> FindTemplateAsync(Guid id);

    Task<Template> UpdateTemplateAsync(Template tmpl);

    Task<Template> CreateTemplateAsync(Template tmpl);

    Task DeleteAllTemplatesAsync(string userName);
}

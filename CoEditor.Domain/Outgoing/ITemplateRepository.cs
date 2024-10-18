using CoEditor.Domain.Model;

namespace CoEditor.Domain.Outgoing;

public interface ITemplateRepository
{
    Task<Template[]> GetTemplatesAsync(string userName, Language language);
}

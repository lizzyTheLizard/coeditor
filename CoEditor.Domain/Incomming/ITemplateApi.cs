using CoEditor.Domain.Model;

namespace CoEditor.Domain.Incomming;

public interface ITemplateApi
{
    Task<Template[]> GetTemplatesAsync(Language language, string userName);
}

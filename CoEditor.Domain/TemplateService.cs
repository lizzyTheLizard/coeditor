using CoEditor.Domain.Outgoing;
using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;

namespace CoEditor.Domain;

internal class TemplateService(ITemplateRepository templateRepository) : ITemplateApi
{
    public async Task<Template[]> GetTemplatesAsync(Language language, string userName)
    {
        return await templateRepository.GetTemplatesAsync(userName, language);
    }
}

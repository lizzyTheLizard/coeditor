using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;

namespace CoEditor.Domain;

internal class TemplateService(ITemplateRepository templateRepository) : ITemplateApi
{
    public async Task<Template[]> GetTemplatesAsync(Language language, string userName)
    {
        return await templateRepository.GetTemplatesAsync(userName, language);
    }
}

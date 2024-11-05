using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class GetTemplatesUseCase(
    ITemplateRepository templateRepository,
    ILogger<GetTemplatesUseCase> logger) : IGetTemplatesApi
{
    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var userTemplates = await templateRepository.GetTemplatesAsync(userName, language);
        Template[] templates = [.. GetSystemTemplates(userName, language), ..userTemplates];
        logger.TemplatesLoaded(templates, userName, language);
        return templates;
    }

    private static Template[] GetSystemTemplates(string userName, Language language)
    {
        // TODO: Fix for other languages
        // TODO: Fixed GUIDS for system templates
        return
        [
            new Template
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Language = language,
                Name = "Default",
                Text = "I want to write {Context:LongText}",
                DefaultTemplate = true,
            },
            new Template
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Language = language,
                Name = "Email Work Colleague",
                Text =
                    "I want to write an email to a work colleague named {Name:text}. I know him {Knowledge:select:well,barely}. The tone should be {Tone:select:humble,aggressive,friendly}. The content of the mail considers {Context:longtext}",
                DefaultTemplate = true,
            }
        ];
    }
}

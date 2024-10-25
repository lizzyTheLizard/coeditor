using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace CoEditor.Domain.UseCase;

internal class GetTemplatesUseCase(
    ITemplateRepository templateRepository,
    ILogger<GetTemplatesUseCase> logger) : IGetTemplatesApi
{
    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var userTemplates = await templateRepository.GetTemplatesAsync(userName, language);
        Template[] templates = [..userTemplates, .. GetSystemTemplates(language)];
        logger.TemplatesLoaded(templates, userName, language);
        return templates;
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "TODO, will be needed for other languages")]
    private static Template[] GetSystemTemplates(Language language)
    {
        //TODO: Fix for other languages
        //TODO: Fixed GUIDS for system templates
        return
        [
            new Template { Id = Guid.NewGuid(), Name = "Default", Text = "I want to write {Context:LongText}" },
            new Template
            {
                Id = Guid.NewGuid(),
                Name = "Email Work Colleague",
                Text =
                    "I want to write an email to a work colleague named {Name:text}. I know him {Knowledge:select:well,barely}. The tone should be {Tone:select:humble,aggressive,friendly}. The content of the mail considers {Context:longtext}"
            }
        ];
    }
}

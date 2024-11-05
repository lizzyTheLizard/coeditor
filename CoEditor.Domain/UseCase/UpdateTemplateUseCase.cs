using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class UpdateTemplateUseCase(
    ITemplateRepository templateRepository,
    ILogger<UpdateTemplateUseCase> logger) : IUpdateTemplateApi
{
    public async Task<Template> UpdateTemplateAsync(string userName, Template tmpl)
    {
        if (tmpl.UserName != userName)
        {
            throw new ArgumentException("Wrong user name in body");
        }

        var originalTemplate = await templateRepository.FindTemplateAsync(tmpl.Id);
        if (originalTemplate == null)
        {
            var createResult = await templateRepository.CreateTemplateAsync(tmpl);
            logger.TemplateCreated(tmpl);
            return createResult;
        }

        if (originalTemplate.UserName != userName)
        {
            throw new AuthenticationException($"Wrong user {userName} for template {tmpl.Id}");
        }

        var updateResult = await templateRepository.UpdateTemplateAsync(tmpl);
        logger.TemplateUpdated(tmpl);
        return updateResult;
    }
}

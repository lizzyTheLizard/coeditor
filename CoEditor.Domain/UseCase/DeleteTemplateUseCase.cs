using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class DeleteTemplateUseCase(
    ITemplateRepository templateRepository,
    ILogger<DeleteTemplateUseCase> logger) : IDeleteTemplateApi
{
    public async Task DeleteTemplateAsync(string userName, Guid templateId)
    {
        var template = await templateRepository.FindTemplateAsync(templateId);
        if (template == null)
        {
            logger.TemplateAlreadyDeleted(templateId);
            return;
        }

        if (template.UserName != userName)
        {
            throw new AuthenticationException($"Wrong user {userName} for template {templateId}");
        }

        await templateRepository.DeleteTemplateAsync(templateId);
        logger.TemplateDeleted(template);
    }
}

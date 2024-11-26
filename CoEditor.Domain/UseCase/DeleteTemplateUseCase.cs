using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class DeleteTemplateUseCase(
    ITemplateRepository templateRepository,
    IUserService userService,
    ILogger<DeleteTemplateUseCase> logger) : IDeleteTemplateApi
{
    public async Task DeleteTemplateAsync(Guid templateId)
    {
        var userName = await userService.GetUserNameAsync();
        var template = await templateRepository.FindTemplateAsync(templateId);
        if (template == null)
        {
            logger.TemplateAlreadyDeleted(templateId);
            return;
        }

        if (template.UserName != userName)
            throw new AuthenticationException($"Wrong user {userName} for template {templateId}");

        await templateRepository.DeleteTemplateAsync(templateId);
        logger.TemplateDeleted(template);
    }
}

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static partial class DeleteTemplateLogMessages
{
    public static void TemplateDeleted(this ILogger logger, Template template)
    {
        logger.LogInformation(1201, "Deleted template {Id} of user {UserName} in {Language}", template.Id,
            template.UserName, template.Language);
        logger.LogTrace("{Template}", template);
    }

    [LoggerMessage(LogLevel.Information, EventId = 1202, Message = "Template {id} is already deleted")]
    public static partial void TemplateAlreadyDeleted(this ILogger logger, Guid id);
}

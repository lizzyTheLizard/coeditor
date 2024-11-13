using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Services;

public class TemplateService(
    IGetTemplatesApi getTemplatesApi,
    IUpdateTemplateApi updateTemplateApi,
    IDeleteTemplateApi deleteTemplateApi,
    ExceptionService exceptionService,
    UserService userService,
    ILogger<TemplateService> logger)
{
    public async Task UpdateTemplate(Template template)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
            await updateTemplateApi.UpdateTemplateAsync(userName, template);
            logger.TemplateUpdated(template);
        }
        catch (Exception e)
        {
            logger.TemplateUpdateFailed(e, template);
            await exceptionService.HandleException(e);
        }
    }

    public async Task DeleteTemplate(Template template)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
            await deleteTemplateApi.DeleteTemplateAsync(userName, template.Id);
            logger.TemplateDeleted(template);
        }
        catch (Exception e)
        {
            logger.TemplateDeleteFailed(e, template);
            await exceptionService.HandleException(e);
        }
    }

    public async Task CreateTemplate(Template template)
    {
        try
        {
            await updateTemplateApi.UpdateTemplateAsync(template.UserName, template);
            logger.TemplateCreated(template);
        }
        catch (Exception e)
        {
            logger.TemplateCreationFailed(e);
            await exceptionService.HandleException(e);
        }
    }

    public async Task<Template[]> GetTemplatesAsync(Language language)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
            var templates = await getTemplatesApi.GetTemplatesAsync(userName, language);
            logger.TemplatesLoaded(language, templates);
            return templates;
        }
        catch (Exception e)
        {
            logger.TemplatesLoadedFailed(e);
            await exceptionService.HandleException(e);
            return [];
        }
    }

    public async Task<TemplateParameter[]> GetTemplateParameters(Template template)
    {
        try
        {
            return template.GetTemplateParameters();
        }
        catch (Exception e)
        {
            logger.TemplateParametersInvalid(e, template);
            await exceptionService.HandleException(e);
            return [];
        }
    }
}

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class TemplateServiceLogMessages
{
    public static void TemplatesLoaded(this ILogger logger, Language language, Template[] templates)
    {
        logger.LogDebug("Loaded {NbrTemplates} templates ({Language}) for the current user ", templates.Length, language);
        if (!logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }

        foreach (var template in templates)
        {
            logger.LogTrace("{Template}", template);
        }
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2101, Message = "Could not load templates")]
    public static partial void TemplatesLoadedFailed(this ILogger logger, Exception e);

    public static void TemplateParametersInvalid(this ILogger logger, Exception e, Template template)
    {
        logger.LogWarning(2102, e, "Could not load template parameters. Template seems to be invalid.");
        logger.LogTrace("{Template}", template);
    }

    public static void TemplateUpdated(this ILogger logger, Template template)
    {
        logger.LogInformation("Updated template {Id}.", template.Id);
        logger.LogTrace("{Template}", template);
    }

    public static void TemplateUpdateFailed(this ILogger logger, Exception e, Template template)
    {
        logger.LogWarning(2104, e, "Could not update template {Id}.", template.Id);
        logger.LogTrace("{Template}", template);
    }

    public static void TemplateDeleted(this ILogger logger, Template template)
    {
        logger.LogInformation(2105, "Deleted template {Id}.", template.Id);
        logger.LogTrace("{Template}", template);
    }

    public static void TemplateDeleteFailed(this ILogger logger, Exception e, Template template)
    {
        logger.LogWarning(2106, e, "Could not delete template {Id}.", template.Id);
        logger.LogTrace("{Template}", template);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2107, Message = "Could not create template")]
    public static partial void TemplateCreationFailed(this ILogger logger, Exception e);

    public static void TemplateCreated(this ILogger logger, Template template)
    {
        logger.LogInformation(2108, "Updated template {Id}.", template.Id);
        logger.LogTrace("{Template}", template);
    }
}

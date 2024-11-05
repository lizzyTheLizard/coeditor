using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class TemplateService(
    IGetTemplatesApi getTemplatesApi,
    IUpdateTemplateApi updateTemplateApi,
    IDeleteTemplateApi deleteTemplateApi,
    ExceptionService exceptionService,
    AuthenticationStateProvider authenticationStateProvider,
    ILogger<TemplateService> logger)
{
    public async Task UpdateTemplate(Template template)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
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
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            await deleteTemplateApi.DeleteTemplateAsync(userName, template.Id);
            logger.TemplateDeleted(template);
        }
        catch (Exception e)
        {
            logger.TemplateDeleteFailed(e, template);
            await exceptionService.HandleException(e);
        }
    }

    public async Task<Template?> CreateTemplate(Template template)
    {
        try
        {
            await updateTemplateApi.UpdateTemplateAsync(template.UserName, template);
            logger.TemplateCreated(template);
            return template;
        }
        catch (Exception e)
        {
            logger.TemplateCreationFailed(e);
            await exceptionService.HandleException(e);
            return null;
        }
    }

    public async Task<Template[]> GetTemplatesAsync(Language language)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
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

internal static partial class TemplateServiceLogMessages
{
    public static void TemplatesLoaded(this ILogger logger, Language language, Template[] templates)
    {
        logger.TemplatesLoaded(templates.Length, language);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var template in templates)
            logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Loaded {nbrTemplates} templates ({language}) for the current user ")]
    private static partial void TemplatesLoaded(this ILogger logger, int nbrTemplates, Language language);

    [LoggerMessage(LogLevel.Trace, Message = "{template}")]
    private static partial void TraceTemplate(this ILogger logger, Template template);

    [LoggerMessage(LogLevel.Warning, EventId = 2101, Message = "Could not load templates")]
    public static partial void TemplatesLoadedFailed(this ILogger logger, Exception e);

    public static void TemplateParametersInvalid(this ILogger logger, Exception e, Template t)
    {
        logger.TemplateParametersInvalid(e);
        logger.TraceTemplate(t);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2102,
        Message = "Could not load template parameters. Template seems to be invalid.")]
    private static partial void TemplateParametersInvalid(this ILogger logger, Exception e);

    public static void TemplateUpdated(this ILogger logger, Template template)
    {
        logger.TemplateUpdated(template.Id);
        logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2103, Message = "Updated template {id}.")]
    private static partial void TemplateUpdated(this ILogger logger, Guid id);

    public static void TemplateUpdateFailed(this ILogger logger, Exception e, Template template)
    {
        logger.TemplateUpdateFailed(e, template.Id);
        logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2104, Message = "Could not update template {id}")]
    private static partial void TemplateUpdateFailed(this ILogger logger, Exception e, Guid id);

    public static void TemplateDeleted(this ILogger logger, Template template)
    {
        logger.TemplateDeleted(template.Id);
        logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2105, Message = "Deleted template {id}")]
    private static partial void TemplateDeleted(this ILogger logger, Guid id);

    public static void TemplateDeleteFailed(this ILogger logger, Exception e, Template template)
    {
        logger.TemplateDeleteFailed(e, template.Id);
        logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2106, Message = "Could not delete template {id}")]
    private static partial void TemplateDeleteFailed(this ILogger logger, Exception e, Guid id);

    [LoggerMessage(LogLevel.Warning, EventId = 2107, Message = "Could not create template")]
    public static partial void TemplateCreationFailed(this ILogger logger, Exception e);


    [LoggerMessage(LogLevel.Information, EventId = 2108, Message = "Updated template {id}.")]
    private static partial void TemplateCreated(this ILogger logger, Guid id);

    public static void TemplateCreated(this ILogger logger, Template template)
    {
        logger.TemplateCreated(template.Id);
        logger.TraceTemplate(template);
    }
}

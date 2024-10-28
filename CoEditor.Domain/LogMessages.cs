using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain;

internal static partial class LogMessages
{
    #region Profile

    [LoggerMessage(LogLevel.Debug,
        Message = "Loaded {type} profile ({language}) of {userName}. Text size is {textSize}")]
    private static partial void ProfileLoaded(this ILogger logger, string type, string userName, Language language,
        int textSize);

    [LoggerMessage(LogLevel.Trace, Message = "{profile}")]
    private static partial void ProfileTrace(this ILogger logger, Profile profile);

    public static void ProfileLoaded(this ILogger logger, Profile profile, bool defaultProfile, string userName,
        Language language)
    {
        var type = defaultProfile ? "default" : "user";
        logger.ProfileLoaded(type, userName, language, profile.Text.Length);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.ProfileTrace(profile);
    }


    [LoggerMessage(LogLevel.Information, EventId = 1301, Message = "Created profile of user {userName} in {language}")]
    private static partial void ProfileCreated(this ILogger logger, string userName, Language language);

    public static void ProfileCreated(this ILogger logger, Profile profile)
    {
        logger.ProfileCreated(profile.UserName, profile.Language);
        logger.ProfileTrace(profile);
    }

    [LoggerMessage(LogLevel.Information, EventId = 1302, Message = "Updated template of user {userName} in {language}")]
    private static partial void ProfileUpdated(this ILogger logger, string userName, Language language);

    public static void ProfileUpdated(this ILogger logger, Profile profile)
    {
        logger.ProfileUpdated(profile.UserName, profile.Language);
        logger.ProfileTrace(profile);
    }

    #endregion

    #region Template

    [LoggerMessage(LogLevel.Debug, Message = "Loaded {nbrTemplates} templates ({Language}) for {userName}")]
    private static partial void TemplatesLoaded(this ILogger logger, int nbrTemplates, string userName,
        Language language);

    [LoggerMessage(LogLevel.Trace, Message = "{template}")]
    private static partial void TemplateTrace(this ILogger logger, Template template);

    public static void TemplatesLoaded(this ILogger logger, Template[] templates, string userName, Language language)
    {
        logger.TemplatesLoaded(templates.Length, userName, language);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var t in templates)
            logger.TemplateTrace(t);
    }

    [LoggerMessage(LogLevel.Information, EventId = 1201,
        Message = "Deleted template {id} of user {userName} in {language}")]
    private static partial void TemplateDeleted(this ILogger logger, Guid id, string userName, Language language);

    public static void TemplateDeleted(this ILogger logger, Template template)
    {
        logger.TemplateDeleted(template.Id, template.UserName, template.Language);
        logger.TemplateTrace(template);
    }

    [LoggerMessage(LogLevel.Information, EventId = 1202, Message = "Template {id} is already deleted")]
    public static partial void TemplateAlreadyDeleted(this ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Information, EventId = 1203,
        Message = "Created template {id} of user {userName} in {language}")]
    private static partial void TemplateCreated(this ILogger logger, Guid id, string userName, Language language);

    public static void TemplateCreated(this ILogger logger, Template template)
    {
        logger.TemplateCreated(template.Id, template.UserName, template.Language);
        logger.TemplateTrace(template);
    }

    [LoggerMessage(LogLevel.Information, EventId = 1204,
        Message = "Updated template {id} of user {userName} in {language}")]
    private static partial void TemplateUpdated(this ILogger logger, Guid id, string userName, Language language);

    public static void TemplateUpdated(this ILogger logger, Template template)
    {
        logger.TemplateUpdated(template.Id, template.UserName, template.Language);
        logger.TemplateTrace(template);
    }

    #endregion

    #region Conversations

    [LoggerMessage(LogLevel.Debug,
        Message = "Loaded conversation {guid} started at {startedAt}. It has {nbrMessages} messages")]
    private static partial void ConversationLoaded(this ILogger logger, Guid guid, DateTime startedAt, int nbrMessages);

    [LoggerMessage(LogLevel.Information, EventId = 1101,
        Message =
            "Updated conversation {guid} started at {startedAt} with {actionName}. It now has {nbrMessages} messages")]
    private static partial void ConversationUpdated(this ILogger logger, Guid guid, string actionName,
        DateTime startedAt, int nbrMessages);

    [LoggerMessage(LogLevel.Trace, Message = "{conversation}")]
    private static partial void ConversationTrace(this ILogger logger, Conversation conversation);

    public static void ConversationLoaded(this ILogger logger, Conversation conversation)
    {
        logger.ConversationLoaded(conversation.Id, conversation.StartedAt, conversation.Messages.Length);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        ConversationTrace(logger, conversation);
    }

    public static void ConversationUpdated(this ILogger logger, ActionName? actionName, Conversation conversation)
    {
        logger.ConversationUpdated(conversation.Id, actionName?.ToString() ?? "custom action", conversation.StartedAt,
            conversation.Messages.Length);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        ConversationTrace(logger, conversation);
    }

    [LoggerMessage(LogLevel.Debug, EventId = 1102,
        Message = "User {userName} has created conversation {id}. It has {nbrMessages} messages\"")]
    private static partial void ConversationCreated(this ILogger logger, string userName, Guid id, int nbrMessages);

    public static void ConversationCreated(this ILogger logger, Conversation conversation)
    {
        logger.ConversationCreated(conversation.UserName, conversation.Id, conversation.Messages.Length);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.ConversationTrace(conversation);
    }

    #endregion
}

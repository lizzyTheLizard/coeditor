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

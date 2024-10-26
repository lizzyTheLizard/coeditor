using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client;

//TODO: General: remove LoggerMessage for direct calls?
internal static partial class LogMessages
{
    #region UndoService

    [LoggerMessage(LogLevel.Debug, Message = "Text-Change registered into Undo chain")]
    public static partial void TextChangeRegistered(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Undo/Redo chains cleaned")]
    public static partial void Cleaned(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Undone last command")]
    public static partial void Undone(this ILogger logger);

    [LoggerMessage(LogLevel.Debug, Message = "Redone last command")]
    public static partial void Redone(this ILogger logger);

    #endregion

    #region TemplateService

    [LoggerMessage(LogLevel.Debug, Message = "Set template language to {language}")]
    public static partial void TemplateLanguageChanged(this ILogger logger, Language language);

    public static void TemplateChanged(this ILogger logger, Template template)
    {
        logger.TemplateChanged(template.Name, template.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Set Template {name} ({guid})")]
    private static partial void TemplateChanged(this ILogger logger, string name, Guid guid);

    [LoggerMessage(LogLevel.Trace, Message = "{template}")]
    private static partial void TraceTemplate(this ILogger logger, Template template);

    public static void TemplatesLoaded(this ILogger logger, Language language, Template[] templates)
    {
        logger.TemplatesLoaded(templates.Length, language);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var template in templates)
            logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Loaded {nbrTemplates} templates ({language}) for the current user ")]
    private static partial void TemplatesLoaded(this ILogger logger, int nbrTemplates, Language language);

    public static void TemplateParametersNotValid(this ILogger logger, TemplateParameter[] templateParameters)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        var invalidParameters = templateParameters
            .Where(p => !p.Valid)
            .ToList();
        var invalidNames = string.Join(", ", invalidParameters.Select(x => x.Name));
        logger.TemplateParametersNotValid(invalidNames, invalidNames.Length == 1 ? "is" : "are");
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var p in invalidParameters)
            logger.TraceTemplateParameter(p);
    }

    [LoggerMessage(LogLevel.Debug, Message = "The parameter(s) {invalidNames} {areIs} not valid")]
    private static partial void TemplateParametersNotValid(this ILogger logger, string invalidNames, string areIs);

    [LoggerMessage(LogLevel.Trace, Message = "{templateParameter}")]
    private static partial void TraceTemplateParameter(this ILogger logger, TemplateParameter templateParameter);

    public static void TemplateInitiallyValid(this ILogger logger, string context)
    {
        logger.TemplateInitiallyValid();
        logger.TraceContext(context);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Template parameters are now valid")]
    private static partial void TemplateInitiallyValid(this ILogger logger);

    [LoggerMessage(LogLevel.Trace, Message = "{context}")]
    private static partial void TraceContext(this ILogger logger, string context);

    public static void TemplateContextChanged(this ILogger logger, string context)
    {
        logger.TemplateContextChanged();
        logger.TraceContext(context);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Template parameters have changed")]
    private static partial void TemplateContextChanged(this ILogger logger);

    [LoggerMessage(LogLevel.Warning, EventId = 2101, Message = "Could not load templates")]
    public static partial void TemplatesLoadedFailed(this ILogger logger, Exception e);

    [LoggerMessage(LogLevel.Warning, EventId = 2102,
        Message = "Could not load template parameters. Template seems to be invalid.")]
    private static partial void TemplateParametersInvalid(this ILogger logger, Exception e);

    public static void TemplateParametersInvalid(this ILogger logger, Exception e, Template t)
    {
        logger.TemplateParametersInvalid(e);
        logger.TraceTemplate(t);
    }

    #endregion

    #region ShortcutService

    [LoggerMessage(LogLevel.Trace, Message = "Shortcut {s} registered")]
    public static partial void ShortcutRegistered(this ILogger logger, char s);

    [LoggerMessage(LogLevel.Trace, Message = "Shortcut {s} unregistered")]
    public static partial void ShortcutUnregistered(this ILogger logger, char s);
    
    [LoggerMessage(LogLevel.Trace, Message = "Key {s} pressed. {hit}")]
    private static partial void KeypressHandled(this ILogger logger, string s, string hit);

    public static void KeyPress(this ILogger logger, char key, bool alt, bool hit)
    {
        var s = (alt ? "Alt+" : "") + key;
        var hitStr = hit ? "This is a shortcut" : "This is not a shortcut";
        logger.KeypressHandled(s, hitStr);
    }

    #endregion

    #region ConversationService

    public static void ConversationStarted(this ILogger logger, Conversation conversation)
    {
        ConversationStarted(logger, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2201, Message = "New Conversation {id} started")]
    private static partial void ConversationStarted(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Trace, Message = "{conversation}")]
    private static partial void TraceConversation(ILogger logger, Conversation conversation);

    [LoggerMessage(LogLevel.Warning, EventId = 2202, Message = "Could not start conversation")]
    public static partial void ConversationStartFailed(this ILogger logger, Exception e);

    public static void ConversationEnded(this ILogger logger, Conversation conversation)
    {
        ConversationEnded(logger, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2203, Message = "Conversation {id} has ended")]
    private static partial void ConversationEnded(ILogger logger, Guid id);

    public static void ConversationActionApplied(this ILogger logger, ActionName action, Conversation conversation)
    {
        ConversationActionApplied(logger, action, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    [LoggerMessage(LogLevel.Information, EventId = 2204, Message = "Action {action} on conversation {id} applied")]
    private static partial void ConversationActionApplied(ILogger logger, ActionName action, Guid id);

    [LoggerMessage(LogLevel.Warning, EventId = 2205, Message = "Action {action} could not be applied")]
    public static partial void ConversationActionFailed(this ILogger logger, ActionName action, Exception e);

    #endregion

    #region PersistentAuthenticationStateProvider

    [LoggerMessage(LogLevel.Information, EventId = 2301, Message = "Authentication {userName} read from state")]
    public static partial void UserIsAuthenticated(this ILogger logger, string userName);

    [LoggerMessage(LogLevel.Debug, Message = "No Authentication read from state, user is not authenticated")]
    public static partial void UserIsNotAuthenticated(this ILogger logger);

    #endregion
}

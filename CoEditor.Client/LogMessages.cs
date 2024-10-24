using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client;

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

    [LoggerMessage(LogLevel.Debug, Message = "Loaded {nbrTemplates} templates ({language}) for the current user ")]
    private static partial void TemplatesLoaded(this ILogger logger, int nbrTemplates, Language language);

    [LoggerMessage(LogLevel.Trace, Message = "{template}")]
    private static partial void TraceTemplate(this ILogger logger, Template template);

    public static void TemplatesLoaded(this ILogger logger, Language language, Template[] templates)
    {
        logger.TemplatesLoaded(templates.Length, language);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var template in templates)
            logger.TraceTemplate(template);
    }

    [LoggerMessage(LogLevel.Debug, Message = "The parameter(s) {invalidNames} {areIs} not valid")]
    private static partial void ParametersNotValid(this ILogger logger, string invalidNames, string areIs);

    [LoggerMessage(LogLevel.Trace, Message = "{templateParameter}")]
    private static partial void TraceParameter(this ILogger logger, TemplateParameter templateParameter);

    public static void ParametersNotValid(this ILogger logger, TemplateParameter[] templateParameters)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        var invalidParameters = templateParameters
            .Where(p => !p.Valid)
            .ToList();
        var invalidNames = string.Join(", ", invalidParameters.Select(x => x.Name));
        logger.ParametersNotValid(invalidNames, invalidNames.Length == 1 ? "is" : "are");
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        foreach (var p in invalidParameters)
            logger.TraceParameter(p);
    }

    #endregion

    #region ShortcutService

    [LoggerMessage(LogLevel.Trace, Message = "Shortcut {s} registered")]
    public static partial void ShortcutRegistered(this ILogger logger, char s);

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

    [LoggerMessage(LogLevel.Debug, Message = "New Conversation {id} started")]
    private static partial void ConversationStarted(ILogger logger, Guid id);

    [LoggerMessage(LogLevel.Trace, Message = "{conversation}")]
    private static partial void TraceConversation(ILogger logger, Conversation conversation);


    public static void NewConversationStarted(this ILogger logger, Conversation conversation)
    {
        ConversationStarted(logger, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Conversation {id} has ended")]
    private static partial void ConversationEnded(ILogger logger, Guid id);

    public static void ConversationEnded(this ILogger logger, Conversation conversation)
    {
        ConversationEnded(logger, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    [LoggerMessage(LogLevel.Debug, Message = "Action {action} on conversation {id} applied")]
    private static partial void ActionApplied(ILogger logger, ActionName action, Guid id);

    public static void ActionApplied(this ILogger logger, ActionName action, Conversation conversation)
    {
        ActionApplied(logger, action, conversation.Id);
        if (!logger.IsEnabled(LogLevel.Trace)) return;
        TraceConversation(logger, conversation);
    }

    #endregion
}

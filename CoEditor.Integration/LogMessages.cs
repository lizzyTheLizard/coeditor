using CoEditor.Domain.Dependencies;
using Microsoft.Extensions.Logging;

namespace CoEditor.Integration;

// TODO: General: remove LoggerMessage for direct calls?
#pragma warning disable SA1202 // Access musst be checked afterwards
internal static partial class LogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Persisting authentication state for user {userName}")]
    public static partial void UserPersisted(this ILogger logger, string userName);

    [LoggerMessage(LogLevel.Debug, Message = "Persisting non authentication state")]
    public static partial void NoUserPersisted(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, EventId = 4101,
        Message = "Start prompting AI with a prompt size of {promptSize}")]
    private static partial void PromptStartedDebug(this ILogger logger, long promptSize);

    [LoggerMessage(Level = LogLevel.Trace, Message = "{actor}: {message}")]
    private static partial void PromptStartedTrace(this ILogger logger, PromptMessageType actor, string message);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Finished prompting AI in {timeInMs} ms successfully")]
    private static partial void PromptFinished(ILogger logger, long timeInMs);

    [LoggerMessage(Level = LogLevel.Trace, Message = "{response}")]
    private static partial void PromptFinishedTrace(ILogger logger, string response);

    public static void PromptStarted(this ILogger logger, PromptMessage[] messages)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        var promptSize = messages.Select(m => m.Prompt.Length).Sum();
        PromptStartedDebug(logger, promptSize);
        if (!logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }

        foreach (var message in messages)
        {
            PromptStartedTrace(logger, message.Type, message.Prompt);
        }
    }

    public static void PromptFinished(this ILogger logger, string response, long elapsedMs)
    {
        if (!logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        PromptFinished(logger, elapsedMs);
        if (!logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }

        PromptFinishedTrace(logger, response);
    }
}

using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class ConversationService(
    IInitializeConversationApi initializeConversationApi,
    IHandleActionApi handleActionApi,
    AuthenticationStateProvider authenticationStateProvider,
    ExceptionService exceptionService,
    ILogger<ConversationService> logger)
{
    public async Task<Conversation?> InitializeConversationAsync(Language language, string context)
    {
        var input = new InitializeConversationInput
        {
            Language = language, NewContext = context, NewText = "", ConversationGuid = Guid.NewGuid()
        };
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            var newConversation = await initializeConversationApi.InitializeConversationAsync(userName, input);
            logger.ConversationStarted(newConversation);
            return newConversation;
        }
        catch (Exception e)
        {
            logger.ConversationStartFailed(e);
            await exceptionService.HandleException(e);
            return null;
        }
    }

    public async Task<Conversation?> ApplyActionAsync(HandleNamedActionInput input)
    {
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            var updatedConversation = await handleActionApi.HandleActionAsync(userName, input);
            logger.ConversationActionApplied(input.Action, updatedConversation);
            return updatedConversation;
        }
        catch (Exception e)
        {
            logger.ConversationActionFailed(input.Action, e);
            await exceptionService.HandleException(e);
            return null;
        }
    }
}

internal static partial class ConversationServiceLogMessages
{
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
}

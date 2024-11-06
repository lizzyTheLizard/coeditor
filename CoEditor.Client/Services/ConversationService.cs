using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class ConversationService(
    IInitializeConversationApi initializeConversationApi,
    IHandleActionApi handleActionApi,
    UserService userService,
    ExceptionService exceptionService,
    ILogger<ConversationService> logger)
{
    public async Task<Conversation?> InitializeConversationAsync(Language language, string context)
    {
        var input = new InitializeConversationInput
        {
            Language = language, NewContext = context, NewText = string.Empty, ConversationGuid = Guid.NewGuid(),
        };
        try
        {
            var userName = await userService.GetUserNameAsync();
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
            var userName = await userService.GetUserNameAsync();
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

#pragma warning disable SA1402 // LogMessages are only used in this file
internal static partial class ConversationServiceLogMessages
{
    public static void ConversationStarted(this ILogger logger, Conversation conversation)
    {
        logger.LogInformation(2201, "New Conversation {Id} started", conversation.Id);
        logger.LogTrace("{Conversation}", conversation);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2202, Message = "Could not start conversation")]
    public static partial void ConversationStartFailed(this ILogger logger, Exception e);

    public static void ConversationActionApplied(this ILogger logger, ActionName action, Conversation conversation)
    {
        logger.LogInformation(2204, "Action {Action} on conversation {Id} applied", action, conversation.Id);
        logger.LogTrace("{Conversation}", conversation);
    }

    [LoggerMessage(LogLevel.Warning, EventId = 2205, Message = "Action {action} could not be applied")]
    public static partial void ConversationActionFailed(this ILogger logger, ActionName action, Exception e);
}

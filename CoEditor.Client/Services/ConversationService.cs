using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

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
            Language = language,
            NewContext = context,
            NewText = string.Empty,
            ConversationGuid = Guid.NewGuid()
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

    public async Task<Conversation?> ApplyActionAsync(HandleActionInput input)
    {
        try
        {
            var userName = await userService.GetUserNameAsync();
            var updatedConversation = await handleActionApi.HandleActionAsync(userName, input);
            logger.ConversationActionApplied(input, updatedConversation);
            return updatedConversation;
        }
        catch (Exception e)
        {
            logger.ConversationActionFailed(input, e);
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

    public static void ConversationActionApplied(this ILogger logger, HandleActionInput input, Conversation conversation)
    {
        switch (input)
        {
            case HandleNamedActionInput named:
                logger.LogInformation(2204, "Action {Action} on conversation {Id} applied", named.Action, conversation.Id);
                break;
            case HandleCustomActionInput custom:
                logger.LogInformation(2204, "Custom Action on conversation {Id} applied: {Action}", conversation.Id, custom.Action);
                break;
        }

        logger.LogTrace("{Conversation}", conversation);
    }

    public static void ConversationActionFailed(this ILogger logger, HandleActionInput input, Exception e)
    {
        switch (input)
        {
            case HandleNamedActionInput named:
                logger.LogWarning(2205, e, "Action {Action} could not be applied", named.Action);
                break;
            case HandleCustomActionInput custom:
                logger.LogWarning(2205, e, "Custom action could not be applied:  {Action}", custom.Action);
                break;
        }
    }
}

using System.Security.Authentication;
using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class HandleActionUseCase(
    IAiConnector aiConnector,
    PromptMessageFactory promptMessageFactory,
    IConversationRepository conversationRepository,
    ILogger<HandleActionUseCase> logger) : IHandleActionApi
{
    public async Task<Conversation> HandleActionAsync(string userName, HandleActionInput input)
    {
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);

        if (input is HandleNamedActionInput { Selection: not null } namedInput1)
            conversation = conversation.Update(namedInput1.Selection, input.NewText);
        else
            conversation = conversation.Update(input).Update(newMessages, result);
        await conversationRepository.UpdateAsync(conversation);

        if (input is HandleNamedActionInput namedInput2)
            logger.ConversationUpdated(namedInput2.Action, conversation);
        else
            logger.ConversationUpdated(null, conversation);
        return conversation;
    }

    private async Task<Conversation> GetExistingConversation(string userName, Guid conversationGuid)
    {
        var conversation = await conversationRepository.GetAsync(conversationGuid);
        if (conversation.UserName != userName)
            throw new AuthenticationException($"Wrong user {userName} for conversation {conversationGuid}");

        logger.ConversationLoaded(conversation);
        return conversation;
    }
}

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static partial class InitializeConversationLogMessages
{
    public static void ConversationLoaded(this ILogger logger, Conversation conversation)
    {
        logger.LogDebug(
            "User {UserName} has loaded conversation {Id} started as {StartedAt}. It has {NbrMessages} messages",
            conversation.UserName,
            conversation.Id,
            conversation.StartedAt,
            conversation.Messages.Length);
        logger.LogTrace("{Conversation}", conversation);
    }

    public static void ConversationUpdated(this ILogger logger, ActionName? actionName, Conversation conversation)
    {
        logger.LogInformation(
            1101,
            "User {UserName} has updated conversation {Id} started as {StartedAt} with {Action}. It has now {NbrMessages} messages",
            conversation.UserName,
            conversation.Id,
            conversation.StartedAt,
            actionName?.ToString() ?? "a custom action",
            conversation.Messages.Length);
        logger.LogTrace("{Conversation}", conversation);
    }
}

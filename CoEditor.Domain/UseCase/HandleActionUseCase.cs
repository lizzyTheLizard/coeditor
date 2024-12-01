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
    IUserService userService,
    ILogger<HandleActionUseCase> logger) : IHandleActionApi
{
    public async Task<Conversation> HandleActionAsync(HandleActionInput input)
    {
        var userName = await userService.GetUserNameAsync();
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var newText = GetNewText(input, result);
        conversation = conversation
            .UpdateTextAndContext(newText, input.NewContext)
            .UpdateMessages(newMessages, result);
        await conversationRepository.UpdateAsync(conversation);

        if (input is HandleNamedActionInput namedInput)
            logger.ConversationUpdated(namedInput.Action, conversation);
        else
            logger.ConversationUpdated(null, conversation);
        return conversation;
    }

    private static string GetNewText(HandleActionInput input, PromptResult result)
    {
        if (result.Response is null)
            return input.NewText;
        if (input is not HandleNamedActionInput { Selection: not null } namedInput)
            return result.Response.Trim();
        return input.NewText[..namedInput.Selection.Start] + result.Response.Trim() +
               input.NewText[namedInput.Selection.End..];
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

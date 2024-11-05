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
    public async Task<Conversation> HandleActionAsync(string userName, HandleNamedActionInput input)
    {
        var prompt = promptMessageFactory.GetCommandPrompt(input);
        var customActionInput = new HandleCustomActionInput
        {
            ConversationGuid = input.ConversationGuid,
            NewContext = input.NewContext,
            NewText = input.NewText,
            Action = prompt,
        };
        var result = await HandleActionInternalAsync(userName, customActionInput);
        logger.ConversationUpdated(input.Action, result);
        return result;
    }

    public async Task<Conversation> HandleActionAsync(string userName, HandleCustomActionInput input)
    {
        var result = await HandleActionInternalAsync(userName, input);
        logger.ConversationUpdated(null, result);
        return result;
    }

    public async Task<Conversation> HandleActionInternalAsync(string userName, HandleCustomActionInput input)
    {
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var updatedConversation = conversation.Update(input).Update(newMessages, result);
        await conversationRepository.UpdateAsync(updatedConversation);
        return updatedConversation;
    }

    private async Task<Conversation> GetExistingConversation(string userName, Guid conversationGuid)
    {
        var conversation = await conversationRepository.GetAsync(conversationGuid);
        if (conversation.UserName != userName)
        {
            throw new AuthenticationException($"Wrong user {userName} for conversation {conversationGuid}");
        }

        logger.ConversationLoaded(conversation);
        return conversation;
    }
}

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
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var prompt = promptMessageFactory.GetCommandPrompt(input);
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input, prompt);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var updatedConversation = conversation.Update(input).Update(newMessages, result);
        if (input.Selection != null)
        {
            updatedConversation = updatedConversation.Update(input.Selection, input.NewText);
        }
        await conversationRepository.UpdateAsync(updatedConversation);
        logger.ConversationUpdated(input.Action, updatedConversation);
        return updatedConversation;
    }

    public async Task<Conversation> HandleActionAsync(string userName, HandleCustomActionInput input)
    {
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input, input.Action);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var updatedConversation = conversation.Update(input).Update(newMessages, result);
        await conversationRepository.UpdateAsync(updatedConversation);
        logger.ConversationUpdated(null, updatedConversation);
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

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
    public async Task<Conversation> HandleActionAsync(HandleNamedActionInput input)
    {
        var prompt = promptMessageFactory.GetCommandPrompt(input);
        var customActionInput = new HandleCustomActionInput
        {
            ConversationGuid = input.ConversationGuid,
            NewContext = input.NewContext,
            NewText = input.NewText,
            Action = prompt
        };
        var result = await HandleActionInternalAsync(customActionInput);
        logger.ConversationUpdated(input.Action, result);
        return result;
    }

    public async Task<Conversation> HandleActionAsync(HandleCustomActionInput input)
    {
        var result = await HandleActionInternalAsync(input);
        logger.ConversationUpdated(null, result);
        return result;
    }

    public async Task<Conversation> HandleActionInternalAsync(HandleCustomActionInput input)
    {
        var conversation = await GetExistingConversation(input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input);
        var result = await aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var updatedConversation = conversation.Update(input).Update(newMessages, result);
        await conversationRepository.UpdateAsync(updatedConversation);
        return updatedConversation;
    }

    private async Task<Conversation> GetExistingConversation(Guid conversationGuid)
    {
        var conversation = await conversationRepository.GetAsync(conversationGuid);
        logger.ConversationLoaded(conversation);
        return conversation;
    }
}

using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;

namespace CoEditor.Domain;

internal class ConversationService(
    IProfileRepository _profileRepository,
    IAiConnector _aiConnector,
    PromptMessageFactory promptMessageFactory,
    IConversationRepository _conversationRepository) : IConversationApi
{
    public async Task<Conversation> InitializeConversationAsync(string userName, HandleInitialActionInput input)
    {
        var conversation = Conversation.InitialConversation(userName, input);
        await _conversationRepository.EnsureNotExistingAsync(conversation.Guid);
        var profile = await _profileRepository.GetProfileAsync(conversation.UserName, conversation.Language);
        var messages = promptMessageFactory.GenerateInitialMessages(conversation, profile);
        var result = await _aiConnector.PromptAsync(messages);
        var updatedConversation = conversation.UpdateConversation(messages, result, input);
        await _conversationRepository.CreateAsync(updatedConversation);
        return updatedConversation;
    }

    public async Task<Conversation> HandleActionAsync(string userName, HandleNamedActionInput input)
    {
        var prompt = promptMessageFactory.GetCommandPrompt(input);
        var customActionInput = new HandleCustomActionInput()
        {
            ConversationGuid = input.ConversationGuid,
            NewContext = input.NewContext,
            NewText = input.NewText,
            Action = prompt
        };
        return await HandleActionAsync(userName, customActionInput);
    }

    public async Task<Conversation> HandleActionAsync(string userName, HandleCustomActionInput input)
    {
        var conversation = await GetExistingConversation(userName, input.ConversationGuid);
        var existingMessages = conversation.ToPromptMessages();
        var newMessages = promptMessageFactory.GenerateActionMessages(conversation, input);
        var result = await _aiConnector.PromptAsync([.. existingMessages, .. newMessages]);
        var updatedConversation = conversation.UpdateConversation(newMessages, result, input);
        await _conversationRepository.UpdateAsync(updatedConversation);
        return updatedConversation;
    }

    private async Task<Conversation> GetExistingConversation(string userName, Guid conversationGuid)
    {
        var conversation = await _conversationRepository.GetAsync(conversationGuid);
        if (conversation.UserName != userName) throw new Exception("Wrong User");
        return conversation;
    }
}

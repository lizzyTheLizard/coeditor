using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class InitializeConversationUseCase(
    IAiConnector aiConnector,
    PromptMessageFactory promptMessageFactory,
    IConversationRepository conversationRepository,
    GetProfileUseCase getProfileUseCase,
    ILogger<InitializeConversationUseCase> logger) : IInitializeConversationApi
{
    public async Task<Conversation> InitializeConversationAsync(string userName, InitializeConversationInput input)
    {
        var conversation = Conversation.InitialConversation(userName, input);
        await conversationRepository.EnsureNotExistingAsync(conversation.Id);
        var profile = await getProfileUseCase.GetProfileAsync(userName, input.Language);
        var messages = promptMessageFactory.GenerateInitialMessages(conversation, profile);
        var result = await aiConnector.PromptAsync(messages);
        var updatedConversation = conversation.Update(messages, result);
        await conversationRepository.CreateAsync(updatedConversation);
        logger.ConversationCreated(updatedConversation);
        return updatedConversation;
    }
}

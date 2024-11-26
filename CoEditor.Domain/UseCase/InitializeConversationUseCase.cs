using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.Extensions.Logging;

namespace CoEditor.Domain.UseCase;

internal class InitializeConversationUseCase(
    IAiConnector aiConnector,
    PromptMessageFactory promptMessageFactory,
    IConversationRepository conversationRepository,
    IGetProfileApi getProfileApi,
    IUserService userService,
    ILogger<InitializeConversationUseCase> logger) : IInitializeConversationApi
{
    public async Task<Conversation> InitializeConversationAsync(InitializeConversationInput input)
    {
        var userName = await userService.GetUserNameAsync();
        var conversation = new Conversation
        {
            Id = input.ConversationGuid,
            UserName = userName,
            StartedAt = DateTime.Now,
            Language = input.Language,
            Text = input.NewText,
            Context = input.NewContext,
            Messages = []
        };
        await conversationRepository.EnsureNotExistingAsync(conversation.Id);
        var profile = await getProfileApi.GetProfileAsync(input.Language);
        var messages = promptMessageFactory.GenerateInitialMessages(conversation, profile);
        var result = input.NewContext == string.Empty ? null : await aiConnector.PromptAsync(messages);
        var updatedConversation = conversation
            .UpdateTextAndContext(result?.Response ?? string.Empty, input.NewContext)
            .UpdateMessages(messages, result);
        await conversationRepository.CreateAsync(updatedConversation);
        logger.ConversationCreated(updatedConversation);
        return updatedConversation;
    }
}

#pragma warning disable SA1402,SA1204 // LogMessages are only used in this file
internal static partial class InitializeConversationLogMessages
{
    public static void ConversationCreated(this ILogger logger, Conversation conversation)
    {
        logger.LogInformation(
            1102,
            "User {UserName} has created conversation {Id}. It has {NbrMessages} messages",
            conversation.UserName,
            conversation.Id,
            conversation.Messages.Length);
        logger.LogTrace("{Conversation}", conversation);
    }
}

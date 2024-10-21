using CoEditor.Domain.Incomming;
using CoEditor.Domain.Outgoing;

namespace CoEditor.Domain.Model;

public class Conversation
{
    public required Guid Guid { get; init; }
    public required string UserName { get; init; }
    public required DateTime StartedAt { get; init; }
    public required Language Language { get; init; }
    public required string Text { get; init; }
    public required string Context { get; init; }
    public required ConversationMessage[] Messages { get; init; }

    public static Conversation InitialConversation(string userName, HandleInitialActionInput input)
    {
        return new Conversation
        {
            Guid = input.ConversationGuid,
            UserName = userName,
            StartedAt = DateTime.Now,
            Language = input.Language,
            Text = input.NewText,
            Context = input.NewContext,
            Messages = []
        };
    }

    public PromptMessage[] ToPromptMessages()
    {
        var result = new List<PromptMessage>();
        foreach (var conversationMessage in Messages)
        {
            var type = conversationMessage.Type == ConversationMessageType.System
                ? PromptMessageType.System
                : PromptMessageType.User;
            result.Add(new PromptMessage(conversationMessage.Prompt, type));
            if (conversationMessage.Response != null)
                result.Add(new PromptMessage(conversationMessage.Response, PromptMessageType.Assistant));
        }

        return [.. result];
    }

    public Conversation UpdateConversation(PromptMessage[] newMessages, PromptResult promptResult,
        HandleActionInput input)
    {
        return new Conversation
        {
            Guid = Guid,
            UserName = UserName,
            StartedAt = StartedAt,
            Language = Language,
            Text = promptResult.Response ?? input.NewText,
            Context = input.NewContext,
            Messages = [.. Messages, .. ConvertNewMessages(newMessages, promptResult)]
        };
    }

    private static List<ConversationMessage> ConvertNewMessages(PromptMessage[] newMessages, PromptResult promptResult)
    {
        var result = new List<ConversationMessage>();
        for (var i = 0; i < newMessages.Length; i++)
        {
            var message = newMessages[i];
            var isLast = i + 1 == newMessages.Length;
            var type = message.Type == PromptMessageType.System
                ? ConversationMessageType.System
                : ConversationMessageType.User;
            var conversationMessage = new ConversationMessage
            {
                PromtedAt = DateTime.Now,
                Prompt = message.Prompt,
                Type = type,
                Response = isLast ? promptResult.Response : null,
                DurationInMs = isLast ? promptResult.DurationInMs : null,
                Exception = isLast ? promptResult.Exception?.Message : null,
                StackTrace = isLast ? promptResult.Exception?.StackTrace : null
            };
            result.Add(conversationMessage);
        }

        return result;
    }
}

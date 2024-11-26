using System.ComponentModel.DataAnnotations;
using CoEditor.Domain.Dependencies;

namespace CoEditor.Domain.Model;

public class Conversation
{
    public required Guid Id { get; init; }

    [StringLength(FieldLengths.NameMaxLength)]
    public required string UserName { get; init; }

    public required DateTime StartedAt { get; init; }

    public required Language Language { get; init; }

    [StringLength(FieldLengths.TextMaxLength)]
    public required string Text { get; init; }

    [StringLength(FieldLengths.ContextMaxLength)]
    public required string Context { get; init; }

    public required ConversationMessage[] Messages { get; init; }

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

    public Conversation UpdateTextAndContext(string newText, string newContext) => new()
    {
        Id = Id,
        UserName = UserName,
        StartedAt = StartedAt,
        Language = Language,
        Text = newText,
        Context = newContext,
        Messages = Messages
    };

    public Conversation UpdateMessages(PromptMessage[] newMessages, PromptResult? promptResult) => new()
    {
        Id = Id,
        UserName = UserName,
        StartedAt = StartedAt,
        Language = Language,
        Text = Text,
        Context = Context,
        Messages = [.. Messages, .. ConvertNewMessages(newMessages, promptResult)]
    };

    public override string ToString() =>
        $"{base.ToString()}: Id={Id}, UserName={UserName}, StartedAt={StartedAt}, Language={Language}, TextLength={Text.Length}, ContextLegth={Context.Length}, NumberOfMessages={Messages.Length}, Text={Text}, Context={Context}, Messags=[{string.Join<ConversationMessage>(",", Messages)}]";

    private static List<ConversationMessage> ConvertNewMessages(PromptMessage[] newMessages, PromptResult? promptResult)
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
                PromptedAt = DateTime.Now,
                Prompt = message.Prompt,
                Type = type,
                Response = isLast ? promptResult?.Response : null,
                DurationInMs = isLast ? promptResult?.DurationInMs : null
            };
            result.Add(conversationMessage);
        }

        return result;
    }
}

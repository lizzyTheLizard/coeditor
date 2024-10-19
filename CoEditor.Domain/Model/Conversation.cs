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
}

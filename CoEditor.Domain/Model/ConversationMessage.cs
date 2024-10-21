namespace CoEditor.Domain.Model;

public class ConversationMessage
{
    public required DateTime PromtedAt { get; init; }
    public required string Prompt { get; init; }
    public ConversationMessageType Type { get; init; }
    public string? Response { get; init; }
    public long? DurationInMs { get; init; }
    public string? Exception { get; init; }
    public string? StackTrace { get; init; }
}

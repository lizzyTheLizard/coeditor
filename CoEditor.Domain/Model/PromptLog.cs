namespace CoEditor.Domain.Model;

public class PromptLog
{
    public required DateTime PromtedAt { get; init; }
    public required string Prompt { get; init; }
    public string? Exception { get; init; }
    public string? StackTrace { get; init; }
    public string? Response { get; init; }
    public required long Milliseconds { get; init; }
}

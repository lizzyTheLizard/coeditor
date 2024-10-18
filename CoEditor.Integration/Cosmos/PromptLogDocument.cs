namespace CoEditor.Integration.Cosmos;

internal class PromptLogDocument
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required DateTime PromtedAt { get; init; }
    public required string Prompt { get; init; }
    public string? Exception { get; init; }
    public string? StackTrace { get; init; }
    public string? Response { get; init; }
    public required long Milliseconds { get; init; }
}
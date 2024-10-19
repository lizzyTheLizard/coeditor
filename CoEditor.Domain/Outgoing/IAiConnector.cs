using CoEditor.Domain.Model;

namespace CoEditor.Domain.Outgoing;

public interface IAiConnector
{
    Task<PromptResult> PromptAsync(PromptMessage[] newMessages);
}

public record PromptMessage(string Prompt, PromptMessageType Type);

public record PromptResult(string? Response, Exception? exception, long durationInMs);

public enum PromptMessageType
{
    User,
    System,
    Assistant
}

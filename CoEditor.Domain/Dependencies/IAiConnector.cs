namespace CoEditor.Domain.Dependencies;

public enum PromptMessageType
{
    User,
    System,
    Assistant
}

public interface IAiConnector
{
    Task<PromptResult> PromptAsync(PromptMessage[] newMessages);
}

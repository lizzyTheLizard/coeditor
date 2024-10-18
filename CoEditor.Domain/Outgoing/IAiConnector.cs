namespace CoEditor.Domain.Outgoing;

public interface IAiConnector
{
    Task<string> PromptAsync(string prompt);
}

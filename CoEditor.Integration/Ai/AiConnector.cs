using CoEditor.Domain.Outgoing;
using Microsoft.Extensions.Logging;

namespace CoEditor.Integration.Ai;

internal class AiConnector(ILogger<AiConnector> _logger) : IAiConnector
{
    public async Task<string> PromptAsync(string prompt)
    {
        _logger.LogInformation("Send prompt to AI: {}", prompt);
        //TODO: Implement AI
        await Task.FromException(new Exception("Could not reach service!"));
        return "Replacement";
    }
}

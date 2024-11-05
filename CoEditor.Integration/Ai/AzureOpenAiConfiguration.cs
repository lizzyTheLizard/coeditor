namespace CoEditor.Integration.Ai;

internal class AzureOpenAiConfiguration
{
    public required string Endpoint { get; init; }

    public required string ApiKey { get; init; }

    public required string Model { get; init; }
}

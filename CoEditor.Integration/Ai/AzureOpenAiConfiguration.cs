namespace CoEditor.Integration.Ai;

internal class AzureOpenAiConfiguration
{
    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
    public required string Model { get; set; }
}

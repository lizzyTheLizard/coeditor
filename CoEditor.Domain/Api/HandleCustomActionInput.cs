namespace CoEditor.Domain.Api;

public class HandleCustomActionInput : HandleActionInput
{
    public required string Action { get; init; }
}

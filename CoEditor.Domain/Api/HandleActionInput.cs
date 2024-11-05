namespace CoEditor.Domain.Api;

public class HandleActionInput
{
    public required Guid ConversationGuid { get; init; }

    public required string NewContext { get; init; }

    public required string NewText { get; init; }
}

using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public class InitializeConversationInput
{
    public required Guid ConversationGuid { get; init; }

    public required string NewContext { get; init; }

    public required string NewText { get; init; }

    public required Language Language { get; init; }
}

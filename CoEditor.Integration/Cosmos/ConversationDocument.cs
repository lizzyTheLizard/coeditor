using CoEditor.Domain.Model;

namespace CoEditor.Integration.Cosmos;

internal class ConversationDocument
{
    public required Guid Id { get; init; }

    public required string UserName { get; init; }

    public required DateTime StartedAt { get; init; }

    public required Language Language { get; init; }

    public required string Text { get; set; }

    public required string Context { get; set; }

    public required List<ConversationMessage> Messages { get; set; }
}

using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IInitializeConversationApi
{
    Task<Conversation> InitializeConversationAsync(string userName, InitializeConversationInput input);
}

public class InitializeConversationInput
{
    public required Guid ConversationGuid { get; init; }
    public required string NewContext { get; init; }
    public required string NewText { get; init; }
    public required Language Language { get; init; }
}

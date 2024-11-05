using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IInitializeConversationApi
{
    Task<Conversation> InitializeConversationAsync(string userName, InitializeConversationInput input);
}

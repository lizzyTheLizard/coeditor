using CoEditor.Domain.Model;

namespace CoEditor.Domain.Incomming;

public interface IConversationApi
{
    Task<Conversation> InitializeConversationAsync(string userName, HandleInitialActionInput input);

    Task<Conversation> HandleActionAsync(string userName, HandleNamedActionInput input);

    Task<Conversation> HandleActionAsync(string userName, HandleCustomActionInput input);
}

using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IHandleActionApi
{
    Task<Conversation> HandleActionAsync(string userName, HandleActionInput input);
}

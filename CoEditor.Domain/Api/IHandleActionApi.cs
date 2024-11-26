using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IHandleActionApi
{
    Task<Conversation> HandleActionAsync(HandleActionInput input);
}

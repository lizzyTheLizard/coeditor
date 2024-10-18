using CoEditor.Domain.Model;

namespace CoEditor.Domain.Outgoing;

public interface IPromptLogRepository
{
    Task StoreAsync(string userName, PromptLog chatMessage);

}

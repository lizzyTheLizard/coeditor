using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;

namespace CoEditor.Integration.Cosmos;

internal class PromptLogRepository(CosmosDbContext _dbContext) : IPromptLogRepository
{
    public async Task StoreAsync(string userName, PromptLog chatMessage)
    {
        var promptLogDocument = new PromptLogDocument()
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            PromtedAt = chatMessage.PromtedAt,
            Prompt = chatMessage.Prompt,
            Response = chatMessage.Response,
            Exception = chatMessage.Exception,
            StackTrace = chatMessage.StackTrace,
            Milliseconds = chatMessage.Milliseconds
        };
        _dbContext.Add(promptLogDocument);
        await _dbContext.SaveChangesAsync();
    }
}

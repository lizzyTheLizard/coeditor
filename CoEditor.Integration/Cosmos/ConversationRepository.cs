using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class ConversationRepository(CosmosDbContext _dbContext) : IConversationRepository
{
    public async Task CreateAsync(Conversation conversation)
    {
        var document = new ConversationDocument
        {
            Id = conversation.Guid,
            UserName = conversation.UserName,
            StartedAt = conversation.StartedAt,
            Language = conversation.Language,
            Text = conversation.Text,
            Context = conversation.Context,
            Messages = [.. conversation.Messages]
        };
        _dbContext.Add(document);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EnsureNotExistingAsync(Guid conversationGuid)
    {
        var conversation = await _dbContext.Conversations
            .Where(t => t.Id == conversationGuid)
            .FirstOrDefaultAsync();
        if (conversation != null) throw new Exception("Conversation already exists");
    }

    public async Task<Conversation> GetAsync(Guid conversationGuid)
    {
        var conversation = await _dbContext.Conversations
            .Where(t => t.Id == conversationGuid)
            .FirstOrDefaultAsync() ?? throw new Exception("Conversation not found");
        return new Conversation
        {
            Guid = conversation.Id,
            UserName = conversation.UserName,
            StartedAt = conversation.StartedAt,
            Language = conversation.Language,
            Text = conversation.Text,
            Context = conversation.Context,
            Messages = [.. conversation.Messages]
        };
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        var existing = await _dbContext.Conversations
            .Where(t => t.Id == conversation.Guid)
            .FirstOrDefaultAsync() ?? throw new Exception("Conversation not found");
        existing.Text = conversation.Text;
        existing.Context = conversation.Context;
        existing.Messages = [.. conversation.Messages];
        _dbContext.Update(existing);
        await _dbContext.SaveChangesAsync();
    }
}

using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CoEditor.Integration.Cosmos;

internal class ConversationRepository(CosmosDbContext dbContext)
    : IConversationRepository
{
    public async Task CreateAsync(Conversation conversation)
    {
        var document = new ConversationDocument
        {
            Id = conversation.Id,
            UserName = conversation.UserName,
            StartedAt = conversation.StartedAt,
            Language = conversation.Language,
            Text = conversation.Text,
            Context = conversation.Context,
            Messages = [.. conversation.Messages],
        };
        dbContext.Add(document);
        await dbContext.SaveChangesAsync();
    }

    public async Task EnsureNotExistingAsync(Guid conversationGuid)
    {
        var conversation = await dbContext.Conversations
            .Where(t => t.Id == conversationGuid)
            .Include(conversationDocument => conversationDocument.Messages)
            .SingleOrDefaultAsync();
        if (conversation == null) return;

        throw CosmosException.AlreadyPresentException(typeof(Conversation), conversationGuid);
    }

    public async Task<Conversation> GetAsync(Guid conversationGuid)
    {
        var conversation = await dbContext.Conversations
            .Where(t => t.Id == conversationGuid)
            .Include(conversationDocument => conversationDocument.Messages)
            .SingleAsync() ?? throw CosmosException.NotFoundException(typeof(Conversation), conversationGuid);
        var result = new Conversation
        {
            Id = conversation.Id,
            UserName = conversation.UserName,
            StartedAt = conversation.StartedAt,
            Language = conversation.Language,
            Text = conversation.Text,
            Context = conversation.Context,
            Messages = [.. conversation.Messages],
        };
        return result;
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        var existing = await dbContext.Conversations
            .Where(t => t.Id == conversation.Id)
            .SingleAsync() ?? throw CosmosException.NotFoundException(typeof(Conversation), conversation.Id);
        existing.Text = conversation.Text;
        existing.Context = conversation.Context;
        existing.Messages = [.. conversation.Messages];
        dbContext.Update(existing);
        await dbContext.SaveChangesAsync();
    }
}

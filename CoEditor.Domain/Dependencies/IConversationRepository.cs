﻿using CoEditor.Domain.Model;

namespace CoEditor.Domain.Dependencies;

public interface IConversationRepository
{
    Task<Conversation> GetAsync(Guid conversationGuid);

    Task CreateAsync(Conversation conversation);

    Task UpdateAsync(Conversation conversation);

    Task EnsureNotExistingAsync(Guid conversationGuid);

    Task DeleteAllConversationsAsync(string userName);
}

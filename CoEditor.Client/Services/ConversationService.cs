using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class ConversationService(
    IInitializeConversationApi initializeConversationApi,
    IHandleActionApi handleActionApi,
    AuthenticationStateProvider authenticationStateProvider,
    ILogger<ConversationService> logger)
{
    private readonly List<Action<Conversation?>> _registeredCallbacks = [];
    public Conversation? Current { get; private set; }
    public string Text { get; set; } = "";
    public string Context { get; set; } = "";

    public ConversationChangeSubscription RegisterOnConversationChange(Action<Conversation?> callback)
    {
        _registeredCallbacks.Add(callback);
        return new ConversationChangeSubscription(_registeredCallbacks, callback);
    }

    public async Task StartNewConversationAsync(Language language, string context)
    {
        Context = context;
        Text = "";
        var input = new InitializeConversationInput
        {
            Language = language, NewContext = Context, NewText = Text, ConversationGuid = Guid.NewGuid()
        };
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var userName = authenticationState.User.Identity?.Name ?? "";
        var newConversation = await initializeConversationApi.InitializeConversationAsync(userName, input);
        UpdateConversation(newConversation);
        logger.NewConversationStarted(newConversation);
    }

    public void EndConversation()
    {
        if (Current == null) return;
        var oldConversation = Current;
        UpdateConversation(null);
        logger.ConversationEnded(oldConversation);
    }

    public async Task ApplyActionAsync(ActionName action, Selection? selection)
    {
        if (Current == null) return;
        var input = new HandleNamedActionInput
        {
            ConversationGuid = Current.Id,
            Action = action,
            Selection = selection,
            Language = Current.Language,
            NewContext = Context,
            NewText = Text
        };
        var newConversation = await handleActionApi.HandleActionAsync(input);
        UpdateConversation(newConversation);
        logger.ActionApplied(action, newConversation);
    }

    private void UpdateConversation(Conversation? newConversation)
    {
        Current = newConversation;
        Text = newConversation?.Text ?? "";
        Context = newConversation?.Context ?? "";
        foreach (var callback in _registeredCallbacks) callback(newConversation);
    }
}

public readonly struct ConversationChangeSubscription(
    List<Action<Conversation?>> callbacks,
    Action<Conversation?> callback) : IDisposable
{
    public void Dispose()
    {
        callbacks.Remove(callback);
    }
}

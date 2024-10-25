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
        var input = new InitializeConversationInput
        {
            Language = language, NewContext = context, NewText = "", ConversationGuid = Guid.NewGuid()
        };
        try
        {
            var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authenticationState.User.Identity?.Name ?? "";
            var newConversation = await initializeConversationApi.InitializeConversationAsync(userName, input);
            logger.ConversationStarted(newConversation);
            UpdateConversation(newConversation);
        }
        catch (Exception e)
        {
            //TODO: Error Handling: Show error to user!
            logger.ConversationStartFailed(e);
        }
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
        try
        {
            var updatedConversation = await handleActionApi.HandleActionAsync(input);
            UpdateConversation(updatedConversation);
            logger.ConversationActionApplied(action, updatedConversation);
        }
        catch (Exception e)
        {
            //TODO: Error Handling: Show error to user!
            logger.ConversationActionFailed(action, e);
        }
    }

    private void UpdateConversation(Conversation? newConversation)
    {
        Current = newConversation;
        Text = newConversation?.Text ?? "";
        Context = newConversation?.Context ?? "";
        foreach (var callback in _registeredCallbacks) callback(newConversation);
    }
}

public record ConversationChangeSubscription(
    List<Action<Conversation?>> Callbacks,
    Action<Conversation?> Callback) : IDisposable
{
    public void Dispose()
    {
        Callbacks.Remove(Callback);
        GC.SuppressFinalize(this);
    }
}

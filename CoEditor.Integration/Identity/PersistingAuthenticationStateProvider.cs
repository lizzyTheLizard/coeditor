using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;

namespace CoEditor.Integration.Identity;

//For each authentication, the user name is persisted in the browser's local storage.
internal sealed class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;

    public PersistingAuthenticationStateProvider(
        PersistentComponentState persistentComponentState)
    {
        _state = persistentComponentState;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    void IDisposable.Dispose()
    {
        _subscription.Dispose();
    }

    private async Task OnPersistingAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        var identity = authenticationState.User?.Identity;
        if (identity == null) return;
        if (!identity.IsAuthenticated) return;
        var userName = identity.Name;
        _state.PersistAsJson("UserName", userName);
    }
}

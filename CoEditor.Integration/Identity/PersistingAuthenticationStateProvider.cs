using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace CoEditor.Integration.Identity;

// For each authentication, the userName is persisted in the browser's
// local storage so it can be used from WebAssembly and Server
internal sealed class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly ILogger<PersistingAuthenticationStateProvider> _logger;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;

    public PersistingAuthenticationStateProvider(
        PersistentComponentState state,
        ILogger<PersistingAuthenticationStateProvider> logger)
    {
        _logger = logger;
        _state = state;
        _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    void IDisposable.Dispose()
    {
        _subscription.Dispose();
    }

    private async Task OnPersistingAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        var identity = authenticationState.User.Identity;
        if (identity == null) return;
        if (!identity.IsAuthenticated) return;
        var userName = identity.Name;
        if (userName == null)
        {
            _logger.NoUserPersisted();
            return;
        }

        _logger.UserPersisted(userName);
        _state.PersistAsJson("UserName", userName);
    }
}

#pragma warning disable SA1402, SA1204 // LogMessages are only used in this file
internal static partial class PersistingAuthenticationStateProviderLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Persisting authentication state for user {userName}")]
    public static partial void UserPersisted(this ILogger logger, string userName);

    [LoggerMessage(LogLevel.Debug, Message = "Persisting non authentication state")]
    public static partial void NoUserPersisted(this ILogger logger);
}

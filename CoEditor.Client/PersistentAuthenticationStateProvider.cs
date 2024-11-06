using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client;

// The authentication is done in the backend, the username is stored in the PersistentComponentState and needs to be read by the client
internal class PersistentAuthenticationStateProvider(
    PersistentComponentState state,
    ILogger<PersistentAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private AuthenticationState? _authenticationState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_authenticationState != null)
        {
            return Task.FromResult(_authenticationState);
        }

        var identity = GetIdentity();
        var principal = new ClaimsPrincipal(identity);
        _authenticationState = new AuthenticationState(principal);
        return Task.FromResult(_authenticationState);
    }

    private ClaimsIdentity GetIdentity()
    {
        if (!state.TryTakeFromJson<string>("UserName", out var username) || username is null)
        {
            logger.UserIsNotAuthenticated();
            return new ClaimsIdentity();
        }

        logger.UserIsAuthenticated(username);
        var claims = new Claim[] { new(ClaimTypes.Name, username) };
        return new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider));
    }
}

#pragma warning disable SA1402, SA1204 // LogMessages are only used in this file
internal static partial class PersistentAuthenticationStateProviderLogMessages
{
    [LoggerMessage(LogLevel.Information, EventId = 2301, Message = "Authentication {userName} read from state")]
    public static partial void UserIsAuthenticated(this ILogger logger, string userName);

    [LoggerMessage(LogLevel.Debug, Message = "No Authentication read from state, user is not authenticated")]
    public static partial void UserIsNotAuthenticated(this ILogger logger);
}

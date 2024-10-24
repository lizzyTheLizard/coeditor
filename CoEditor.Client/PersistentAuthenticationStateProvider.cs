using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CoEditor.Client;

internal partial class PersistentAuthenticationStateProvider(
    PersistentComponentState state,
    ILogger<PersistentAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = GetIdentity();
        var principal = new ClaimsPrincipal(identity);
        var authenticationState = new AuthenticationState(principal);
        return Task.FromResult(authenticationState);
    }

    private ClaimsIdentity GetIdentity()
    {
        if (!state.TryTakeFromJson<string>("UserName", out var username) || username is null)
        {
            UserIsNotAuthenticated(logger);
            return new ClaimsIdentity();
        }

        UserIsAuthenticated(logger, username);
        var claims = new Claim[] { new(ClaimTypes.Name, username) };
        return new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider));
    }

    #region Log

    [LoggerMessage(LogLevel.Debug, EventId = 2000,
        Message = "Authentication {userName} read from state")]
    private static partial void UserIsAuthenticated(ILogger logger, string userName);

    [LoggerMessage(LogLevel.Debug, EventId = 2001,
        Message = "No Authentication read from state, user is not authenticated")]
    private static partial void UserIsNotAuthenticated(ILogger logger);

    #endregion
}

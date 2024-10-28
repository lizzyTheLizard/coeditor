using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CoEditor.Client;

internal class PersistentAuthenticationStateProvider(
    PersistentComponentState state,
    ILogger<PersistentAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private AuthenticationState? _authenticationState;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_authenticationState != null) return Task.FromResult(_authenticationState);
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

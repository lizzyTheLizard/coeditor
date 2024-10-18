using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CoEditor.Client;

internal class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Task<AuthenticationState> _authenticationStateTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<string>("UserName", out var username) || username is null)
        {
            _authenticationStateTask= Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            return;
        }

        var claims = new Claim[] { new(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider));
        var principal = new ClaimsPrincipal(identity);
        var authenticationState = new AuthenticationState(principal);
        _authenticationStateTask = Task.FromResult(authenticationState);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}


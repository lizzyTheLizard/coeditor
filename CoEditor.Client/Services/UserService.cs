using Microsoft.AspNetCore.Components.Authorization;

namespace CoEditor.Client.Services;

public class UserService(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<string> GetUserNameAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User.Identity?.Name ?? throw new NotAllowedException("User is not authenticated");
    }
}

#pragma warning disable SA1402 // Error is only used in this file
public sealed class NotAllowedException(string message) : Exception(message)
{
}

using CoEditor.Domain.Incomming;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace CoEditor.Client.Services;

public interface IEditorActionService
{
    Task<string> HandleEditorActionAsync(EditorActionInput input);
}

public class EditorActionRestService(HttpClient _httpClient) : IEditorActionService
{
    public async Task<string> HandleEditorActionAsync(EditorActionInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/EditorAction", input);
        if (!response.IsSuccessStatusCode) throw new Exception("Error while handling editor action");
        return await response.Content.ReadFromJsonAsync<string>() ?? throw new Exception("No response from server");
    }
}

public class EditorActionDomainService(AuthenticationStateProvider _authenticationStateProvider, IEditorActionApi _domainEditorActionService) : IEditorActionService
{
    public async Task<string> HandleEditorActionAsync(EditorActionInput input)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = state.User.Identity ?? throw new Exception("Not authenticated");
        var userName = identity.Name ?? throw new Exception("User has no name");
        return await _domainEditorActionService.HandleEditorActionAsync(userName, input);
    }
}


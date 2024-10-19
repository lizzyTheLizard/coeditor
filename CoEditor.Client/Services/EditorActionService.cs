using CoEditor.Domain.Incomming;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace CoEditor.Client.Services;

public interface IEditorActionService
{
    Task<string> InitializeConversationAsync(InitializeConversationInput input);

    Task<string> HandleEditorCommandAsync(HandleEditorCommandInput input);

    Task<string> HandleCustomEditorCommandAsync(HandleCustomEditorCommandInput input);
}

public class EditorActionRestService(HttpClient _httpClient) : IEditorActionService
{
    public async Task<string> InitializeConversationAsync(InitializeConversationInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/EditorAction/Initialize", input);
        return await GetResponse(response);
    }

    public async Task<string> HandleEditorCommandAsync(HandleEditorCommandInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/EditorAction/Action", input);
        return await GetResponse(response);
    }

    public async Task<string> HandleCustomEditorCommandAsync(HandleCustomEditorCommandInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/EditorAction/CustomAction", input);
        return await GetResponse(response);
    }

    private static async Task<string> GetResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) throw new Exception("Error while handling editor action: " + response.StatusCode);
        return await response.Content.ReadAsStringAsync() ?? throw new Exception("No response from server");
    }
}

public class EditorActionDomainService(AuthenticationStateProvider _authenticationStateProvider, IEditorActionApi _domainEditorActionService) : IEditorActionService
{
    public async Task<string> InitializeConversationAsync(InitializeConversationInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.InitializeConversationAsync(userName, input);
    }

    public async Task<string> HandleEditorCommandAsync(HandleEditorCommandInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.HandleEditorCommandAsync(userName, input);
    }

    public async Task<string> HandleCustomEditorCommandAsync(HandleCustomEditorCommandInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.HandleCustomEditorCommandAsync(userName, input);
    }

    private async Task<string> GetUserName()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = state.User.Identity ?? throw new Exception("Not authenticated");
        var userName = identity.Name ?? throw new Exception("User has no name");
        return userName;
    }
}


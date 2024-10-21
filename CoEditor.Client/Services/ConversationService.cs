using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace CoEditor.Client.Services;

public interface IConversationService
{
    Task<Conversation> InitializeConversationAsync(HandleInitialActionInput input);

    Task<Conversation> HandleActionAsync(HandleNamedActionInput input);

    Task<Conversation> HandleActionAsync(HandleCustomActionInput input);
}

public class ConversationRestService(HttpClient _httpClient) : IConversationService
{
    public async Task<Conversation> InitializeConversationAsync(HandleInitialActionInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Conversation/Initialize", input);
        return await GetResponse(response);
    }

    public async Task<Conversation> HandleActionAsync(HandleNamedActionInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Conversation/Action", input);
        return await GetResponse(response);
    }

    public async Task<Conversation> HandleActionAsync(HandleCustomActionInput input)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Conversation/CustomAction", input);
        return await GetResponse(response);
    }

    private static async Task<Conversation> GetResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            throw new Exception("Error while handling editor action: " + response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new Exception("No response from server");
    }
}

public class ConversationDomainService(
    AuthenticationStateProvider _authenticationStateProvider,
    IConversationApi _domainEditorActionService) : IConversationService
{
    public async Task<Conversation> InitializeConversationAsync(HandleInitialActionInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.InitializeConversationAsync(userName, input);
    }

    public async Task<Conversation> HandleActionAsync(HandleNamedActionInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.HandleActionAsync(userName, input);
    }

    public async Task<Conversation> HandleActionAsync(HandleCustomActionInput input)
    {
        var userName = await GetUserName();
        return await _domainEditorActionService.HandleActionAsync(userName, input);
    }

    private async Task<string> GetUserName()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = state.User.Identity ?? throw new Exception("Not authenticated");
        var userName = identity.Name ?? throw new Exception("User has no name");
        return userName;
    }
}

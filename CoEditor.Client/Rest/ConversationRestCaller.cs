using System.Net.Http.Json;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Rest;

public class ConversationRestCaller(HttpClient httpClient) : IInitializeConversationApi, IHandleActionApi
{
    public async Task<Conversation> HandleActionAsync(string userName, HandleNamedActionInput input)
    {
        const string url = "api/Conversation/Action";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
        {
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }

    public async Task<Conversation> HandleActionAsync(string userName, HandleCustomActionInput input)
    {
        const string url = "api/Conversation/CustomAction";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
        {
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }

    public async Task<Conversation> InitializeConversationAsync(string userName, InitializeConversationInput input)
    {
        const string url = "api/Conversation/Initialize";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
        {
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }
}

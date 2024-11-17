using System.Net.Http.Json;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Rest;

public class ConversationRestCaller(HttpClient httpClient) : IInitializeConversationApi, IHandleActionApi
{
    public async Task<Conversation> HandleActionAsync(string userName, HandleActionInput input)
    {
        var url = input switch
        {
            HandleNamedActionInput => "api/Conversation/Action",
            HandleCustomActionInput => "api/Conversation/CustomAction",
            _ => throw new InvalidOperationException($"Not implemented action type {input.GetType()}"),
        };
        var response = await (input switch
        {
            HandleNamedActionInput actionInput => httpClient.PostAsJsonAsync(url, actionInput),
            HandleCustomActionInput actionInput => httpClient.PostAsJsonAsync(url, actionInput),
            _ => throw new InvalidOperationException($"Not implemented action type {input.GetType()}"),
        });
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);

        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }

    public async Task<Conversation> InitializeConversationAsync(string userName, InitializeConversationInput input)
    {
        const string url = "api/Conversation/Initialize";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);

        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }
}

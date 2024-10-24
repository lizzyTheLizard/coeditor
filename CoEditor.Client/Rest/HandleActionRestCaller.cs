using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using System.Net.Http.Json;

namespace CoEditor.Client.Rest;

public class HandleActionRestCaller(HttpClient httpClient) : IHandleActionApi
{
    public async Task<Conversation> HandleActionAsync(HandleNamedActionInput input)
    {
        const string url = "api/Conversation/Action";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }

    public async Task<Conversation> HandleActionAsync(HandleCustomActionInput input)
    {
        const string url = "api/Conversation/CustomAction";
        var response = await httpClient.PostAsJsonAsync(url, input);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Post, url, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Conversation>() ??
               throw new ServiceCallFailedException(HttpMethod.Post, url);
    }
}

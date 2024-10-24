using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using System.Net.Http.Json;

namespace CoEditor.Client.Rest;

public class InitializeConversationRestCaller(HttpClient httpClient) : IInitializeConversationApi
{
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

using CoEditor.Domain.Api;
using CoEditor.Domain.Model;
using System.Net.Http.Json;

namespace CoEditor.Client.Rest;

public class GetTemplatesRestCaller(HttpClient httpClient) : IGetTemplatesApi
{
    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var url = $"api/Template/Mine/{language}";
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Template[]>() ??
               throw new ServiceCallFailedException(HttpMethod.Get, url);
    }
}

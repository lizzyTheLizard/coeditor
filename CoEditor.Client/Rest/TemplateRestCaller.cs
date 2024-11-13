using System.Net.Http.Json;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Rest;

public class TemplateRestCaller(HttpClient httpClient) : IGetTemplatesApi, IUpdateTemplateApi, IDeleteTemplateApi
{
    public async Task DeleteTemplateAsync(string userName, Guid templateId)
    {
        var url = $"api/Template/{templateId}";
        var response = await httpClient.DeleteAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
    }

    public async Task<Template[]> GetTemplatesAsync(string userName, Language language)
    {
        var url = $"api/Template/Mine/{language}";
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Template[]>() ??
               throw new ServiceCallFailedException(HttpMethod.Get, url);
    }

    public async Task<Template> UpdateTemplateAsync(string userName, Template tmpl)
    {
        var url = $"api/Template/{tmpl.Id}";
        var response = await httpClient.PutAsJsonAsync(url, tmpl);
        if (!response.IsSuccessStatusCode)
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Template>() ??
               throw new ServiceCallFailedException(HttpMethod.Put, url);
    }
}

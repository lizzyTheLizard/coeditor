using CoEditor.Domain.Incomming;
using CoEditor.Domain.Model;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace CoEditor.Client.Services;

public interface ITemplateService
{
    Task<Template[]> GetTemplatesAsync(Language language);
}

public class TemplateRestService(HttpClient _httpClient) : ITemplateService
{
    public async Task<Template[]> GetTemplatesAsync(Language language)
    {
        var response = await _httpClient.GetAsync($"api/Template/Mine/{language}");
        if (!response.IsSuccessStatusCode) throw new Exception("Error while handling editor action");
        return await response.Content.ReadFromJsonAsync<Template[]>() ?? throw new Exception("No response from server");
    }
}

public class TemplateDomainService(
    AuthenticationStateProvider _authenticationStateProvider,
    ITemplateApi _domainTemplateService) : ITemplateService
{
    public async Task<Template[]> GetTemplatesAsync(Language language)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var identity = state.User.Identity ?? throw new Exception("Not authenticated");
        var userName = identity.Name ?? throw new Exception("User has no name");
        return await _domainTemplateService.GetTemplatesAsync(language, userName);
    }
}

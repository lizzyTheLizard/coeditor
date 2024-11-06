using System.Net.Http.Json;
using CoEditor.Domain.Api;
using CoEditor.Domain.Model;

namespace CoEditor.Client.Rest;

public class ProfileRestCaller(HttpClient httpClient) : IGetProfileApi, IUpdateProfileApi
{
    public async Task<Profile> GetProfileAsync(string userName, Language language)
    {
        var url = $"api/Profile/Mine/{language}";
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Profile>() ??
               throw new ServiceCallFailedException(HttpMethod.Get, url);
    }

    public async Task<Profile> UpdateProfileAsync(string userName, Profile profile)
    {
        var url = $"api/Profile/Mine/{profile.Language}";
        var response = await httpClient.PutAsJsonAsync(url, profile);
        if (!response.IsSuccessStatusCode)
        {
            throw new ServiceCallFailedException(HttpMethod.Get, url, response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<Profile>() ??
               throw new ServiceCallFailedException(HttpMethod.Put, url);
    }
}

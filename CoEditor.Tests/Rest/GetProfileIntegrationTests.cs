using CoEditor.Components;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CoEditor.Tests.Rest;

public class GetProfileIntegrationTests(WebApplicationFactory<App> factory) : IClassFixture<WebApplicationFactory<App>>
{
    [Fact]
    public async Task NotLoggdIn_RedirectToAbout()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/Profile/Mine/De", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}

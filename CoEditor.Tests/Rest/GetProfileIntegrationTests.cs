using System.Diagnostics.CodeAnalysis;
using CoEditor.Components;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CoEditor.Tests.Rest;

public class GetProfileIntegrationTests(WebApplicationFactory<App> factory) : IClassFixture<WebApplicationFactory<App>>
{
    [Fact]
    [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out",
        Justification = "Test WIP")]
    public async Task NotLoggedIn()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/Profile/Mine/De", TestContext.Current.CancellationToken);
        /* TODO: Not working as this returns 404...
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
       */
        Assert.Equal("0", response.Content.Headers.ContentLength?.ToString());
    }
}

using CoEditor.Components;
using CoEditor.Domain.Dependencies;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CoEditor.Tests.Helper;

public class CustomWebApplicationFactory : WebApplicationFactory<App>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => { services.AddScoped<IUserService, MockUserService>(); });
    }
}

file class MockUserService : IUserService
{
    public Task<string> GetUserNameAsync() => Task.FromResult("TestUser");
}

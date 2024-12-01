using CoEditor.Components;
using CoEditor.Domain.Dependencies;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace CoEditor.Tests.Helper;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;

    public IntegrationTestBase(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        UserName = outputHelper.GetTestName();
        _scope = factory.Services.CreateScope();
        var userService = (MockUserService)_scope.ServiceProvider.GetRequiredService<IUserService>();
        userService.UserName = UserName;
    }

    protected string UserName { get; }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    protected T GetRequiredService<T>()
        where T : notnull
        => _scope.ServiceProvider.GetRequiredService<T>();
}

#pragma warning disable SA1402
public class CustomWebApplicationFactory : WebApplicationFactory<App>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => { services.AddScoped<IUserService, MockUserService>(); });
    }
}

#pragma warning disable SA1402
file class MockUserService : IUserService
{
    public string UserName { get; set; } = "TestUser";

    public Task<string> GetUserNameAsync() => Task.FromResult(UserName);
}

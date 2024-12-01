using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class GetProfileIntegrationTests : IntegrationTestBase
{
    private readonly IProfileRepository _repository;
    private readonly IGetProfileApi _target;

    public GetProfileIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<IProfileRepository>();
        _target = GetRequiredService<IGetProfileApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllProfilesAsync(UserName);
    }

    [Fact]
    public async Task NoProfile_ReturnsDefaultProfile()
    {
        var defaultProfile = await _target.GetProfileAsync(Language.En);

        Assert.NotNull(defaultProfile);
        Assert.Equal(Language.En, defaultProfile.Language);
        Assert.Equal(UserName, defaultProfile.UserName);
        Assert.NotEmpty(defaultProfile.Text);
    }

    [Fact]
    public async Task HasProfile_ReturnsProfile()
    {
        var profile = TestData.Profile(UserName);
        await _repository.CreateProfileAsync(profile);

        var existingProfile = await _target.GetProfileAsync(Language.De);

        CoEditorAssert.Equal(profile, existingProfile);
    }
}

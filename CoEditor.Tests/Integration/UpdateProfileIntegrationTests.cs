using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class UpdateProfileIntegrationTests : IntegrationTestBase
{
    private readonly IProfileRepository _repository;
    private readonly IUpdateProfileApi _target;

    public UpdateProfileIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<IProfileRepository>();
        _target = GetRequiredService<IUpdateProfileApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllProfilesAsync(UserName);
    }

    [Fact]
    public async Task NotExisting_Create()
    {
        var profile = TestData.Profile(UserName);

        var newProfile = await _target.UpdateProfileAsync(profile);

        CoEditorAssert.Equal(profile, newProfile);
        await CoEditorAssert.Contains(profile, _repository);
    }

    [Fact]
    public async Task Existing_Overwrite()
    {
        var profile = TestData.Profile(UserName);
        await _repository.CreateProfileAsync(profile);
        profile.Text = "New Text 222";

        var newProfile = await _target.UpdateProfileAsync(profile);

        CoEditorAssert.Equal(profile, newProfile);
        await CoEditorAssert.Contains(profile, _repository);
    }

    [Fact]
    public async Task Existing_DoNotOverwriteOther()
    {
        var profile = TestData.Profile(UserName);
        await _repository.CreateProfileAsync(profile);
        var profile2 = new Profile
        {
            UserName = UserName,
            Language = Language.En,
            Text = "TestText 222"
        };
        await _repository.CreateProfileAsync(profile2);
        profile.Text = "New Text 222";

        await _target.UpdateProfileAsync(profile);

        await CoEditorAssert.Contains(profile, _repository);
        await CoEditorAssert.Contains(profile2, _repository);
    }
}

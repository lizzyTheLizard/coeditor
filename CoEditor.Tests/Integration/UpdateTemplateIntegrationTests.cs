using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class UpdateTemplateIntegrationTests : IntegrationTestBase
{
    private readonly ITemplateRepository _repository;
    private readonly IUpdateTemplateApi _target;

    public UpdateTemplateIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<ITemplateRepository>();
        _target = GetRequiredService<IUpdateTemplateApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllTemplatesAsync(UserName);
    }

    [Fact]
    public async Task NotExisting_Create()
    {
        var template = TestData.TemplateWithoutParameters(UserName);

        var newTemplate = await _target.UpdateTemplateAsync(template);

        CoEditorAssert.Equal(template, newTemplate);
        await CoEditorAssert.Contains(template, _repository);
    }

    [Fact]
    public async Task Existing_Overwrite()
    {
        var template = TestData.TemplateWithoutParameters(UserName);
        await _repository.CreateTemplateAsync(template);
        template.Text = "New Text 222";

        var newTemplate = await _target.UpdateTemplateAsync(template);

        CoEditorAssert.Equal(template, newTemplate);
        await CoEditorAssert.Contains(template, _repository);
    }

    [Fact]
    public async Task Existing_DoNotOverwriteOther()
    {
        var template = TestData.TemplateWithoutParameters(UserName);
        await _repository.CreateTemplateAsync(template);
        var template2 = TestData.TemplateWithParameters(UserName);
        await _repository.CreateTemplateAsync(template2);
        template.Text = "New Text 222";

        await _target.UpdateTemplateAsync(template);

        await CoEditorAssert.Contains(template, _repository);
        await CoEditorAssert.Contains(template2, _repository);
    }
}

using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class GetTemplatesIntegrationTests : IntegrationTestBase
{
    private readonly ITemplateRepository _repository;
    private readonly IGetTemplatesApi _target;

    public GetTemplatesIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<ITemplateRepository>();
        _target = GetRequiredService<IGetTemplatesApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllTemplatesAsync(UserName);
    }

    [Fact]
    public async Task NoTemplate_ReturnsDefaultTemplates()
    {
        var templates = await _target.GetTemplatesAsync(Language.De);

        Assert.Equal(2, templates.Length);
        for (var i = 0; i < 2; i++)
        {
            Assert.Equal(Language.De, templates[i].Language);
            Assert.Equal(UserName, templates[i].UserName);
            Assert.NotNull(templates[i].Name);
            Assert.NotNull(templates[i].Text);
            Assert.True(templates[i].DefaultTemplate);
        }
    }

    [Fact]
    public async Task HasTemplate_ReturnsAll()
    {
        var template = TestData.TemplateWithoutParameters(UserName);
        await _repository.CreateTemplateAsync(template);

        var existingTemplate = await _target.GetTemplatesAsync(Language.En);

        Assert.Equal(3, existingTemplate.Length);
        CoEditorAssert.Equal(template, existingTemplate[2]);
    }
}

using CoEditor.Domain.Api;
using CoEditor.Domain.Dependencies;
using CoEditor.Tests.Helper;
using Xunit.Abstractions;

namespace CoEditor.Tests.Integration;

public class DeleteTemplateIntegrationTests : IntegrationTestBase
{
    private readonly ITemplateRepository _repository;
    private readonly IDeleteTemplateApi _target;

    public DeleteTemplateIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        _repository = GetRequiredService<ITemplateRepository>();
        _target = GetRequiredService<IDeleteTemplateApi>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await _repository.DeleteAllTemplatesAsync(UserName);
    }

    [Fact]
    public async Task DeleteNormal_IsDeleted()
    {
        var template = TestData.TemplateWithoutParameters(UserName);
        await _repository.CreateTemplateAsync(template);

        await _target.DeleteTemplateAsync(template.Id);

        await CoEditorAssert.DoesNotContain(template, _repository);
    }

    [Fact]
    public async Task AlreadyDeleted_Ignore()
    {
        var template = TestData.TemplateWithoutParameters(UserName);

        await _target.DeleteTemplateAsync(template.Id);

        await CoEditorAssert.DoesNotContain(template, _repository);
    }
}

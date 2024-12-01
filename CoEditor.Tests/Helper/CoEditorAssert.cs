using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using Xunit.Sdk;

namespace CoEditor.Tests.Helper;

public static class CoEditorAssert
{
    public static async Task Contains(Template element, ITemplateRepository repository)
    {
        var allTemplates = await repository.GetTemplatesAsync(element.UserName, element.Language);
        var existing = Array.Find(allTemplates, t => t.Id == element.Id);
        if (existing is null)
            throw FailException.ForFailure($"No template with ID {element.Id} found");
        Assert.Equal(element, existing);
    }

    public static async Task Contains(Profile element, IProfileRepository repository)
    {
        var existing = await repository.FindProfileAsync(element.UserName, element.Language);
        if (existing is null)
            throw FailException.ForFailure(
                $"No profile for user {element.UserName} and Language {element.Language} found");
        Assert.Equal(element, existing);
    }

    public static async Task DoesNotContain(Template element, ITemplateRepository repository)
    {
        var allTemplates = await repository.GetTemplatesAsync(element.UserName, element.Language);
        var existing = Array.Find(allTemplates, t => t.Id == element.Id);
        if (existing is not null)
            throw FailException.ForFailure($"Template with ID {element.Id} found");
    }

    public static void Equal(Template expected, Template actual)
    {
        Assert.Equal(expected.Language, actual.Language);
        Assert.Equal(expected.UserName, actual.UserName);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Text, actual.Text);
        Assert.Equal(expected.DefaultTemplate, actual.DefaultTemplate);
    }

    public static void Equal(Profile expected, Profile actual)
    {
        Assert.Equal(expected.Language, actual.Language);
        Assert.Equal(expected.UserName, actual.UserName);
        Assert.Equal(expected.Text, actual.Text);
    }
}

using CoEditor.Domain.Model;
using CoEditor.Tests.Helper;

namespace CoEditor.Tests.Domain;

public class TemplateUnitTests
{
    [Fact]
    public void GetTemplateParameters_Empty()
    {
        var template = TestData.TemplateWithoutParameters("TestUser");
        var parameters = template.GetTemplateParameters();
        Assert.Empty(parameters);
    }

    [Fact]
    public void GetTemplateParameters_NonEmpty()
    {
        var template = TestData.TemplateWithParameters("TestUser");
        var parameters = template.GetTemplateParameters();
        Assert.Equal(3, parameters.Length);
        Assert.Equal("name", parameters[0].Name);
        Assert.Equal(TemplateParameterType.Text, parameters[0].Type);
        Assert.Empty(parameters[0].Options);
        Assert.Equal("long", parameters[1].Name);
        Assert.Equal(TemplateParameterType.LongText, parameters[1].Type);
        Assert.Empty(parameters[1].Options);
        Assert.Equal("select", parameters[2].Name);
        Assert.Equal(TemplateParameterType.Select, parameters[2].Type);
        Assert.Equal(["option1", "option2"], parameters[2].Options);
    }

    [Fact]
    public void CalculateText()
    {
        var conversation = new Template
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            Language = Language.En,
            Name = "TestName",
            Text = "{name:text} {long:longtext} {select:select:option1,option2}",
            DefaultTemplate = false
        };
        var text = conversation.CalculateText(
        [
            new TemplateParameter
                { Name = "name", Type = TemplateParameterType.Text, Value = "TestValue", Options = [] },
            new TemplateParameter
                { Name = "long", Type = TemplateParameterType.LongText, Value = "TestLongValue", Options = [] },
            new TemplateParameter
                { Name = "select", Type = TemplateParameterType.Select, Value = "option2", Options = [] }
        ]);
        Assert.Equal("TestValue TestLongValue option2", text);
    }
}

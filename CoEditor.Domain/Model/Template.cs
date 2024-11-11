using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CoEditor.Domain.Model;

public partial class Template
{
    public required Guid Id { get; init; }

    public required Language Language { get; init; }

    [StringLength(FieldLengths.NameMaxLength)]
    public required string UserName { get; init; }

    [StringLength(FieldLengths.NameMaxLength)]
    public required string Name { get; init; }

    [StringLength(FieldLengths.ContextMaxLength)]
    public required string Text { get; set; }

    public required bool DefaultTemplate { get; init; }

    public override string ToString()
    {
        return $"{base.ToString()}: Id={Id}, Language={Language}, UserName={UserName}, Name={Name}, Text={Text}, DefaultTemplate={DefaultTemplate}";
    }

    public TemplateParameter[] GetTemplateParameters()
    {
        var matches = TemplateParameterRegex().Matches(Text);
        var parameters = new List<TemplateParameter>();
        foreach (Match match in matches)
        {
            var value = match.Groups["value"].Value;
            var parts = value.Split(':');
            var name = parts[0];
            var type = parts.Length > 1 ? parts[1] : "string";
            var options = parts.Length > 2 ? parts[2].Split(",") : [];
            var parameter = new TemplateParameter
            {
                Name = name,
                Type = Enum.Parse<TemplateParameterType>(type, true),
                Options = options,
            };
            parameters.Add(parameter);
        }

        return [.. parameters];
    }

    public string CalculateText(TemplateParameter[] templateParameters)
    {
        return TemplateParameterRegex().Replace(Text, match =>
        {
            var value = match.Groups["value"].Value;
            var parts = value.Split(':');
            var name = parts[0];
            var parameter = Array.Find(templateParameters, p => p.Name == name) ?? throw new ArgumentOutOfRangeException(name);
            return parameter.Value;
        });
    }

    [GeneratedRegex(@"\{(?<value>[^\}]+)\}")]
    private static partial Regex TemplateParameterRegex();
}

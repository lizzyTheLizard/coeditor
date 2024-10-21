using System.Text.RegularExpressions;

namespace CoEditor.Domain.Model;

public class Template
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Text { get; init; }

    public TemplateParameter[] GetTemplateParameters()
    {
        var parameterPattern = @"\{(?<value>[^\}]+)\}";
        var matches = Regex.Matches(Text, parameterPattern);
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
                Name = name, Type = Enum.Parse<TemplateParameterType>(type, true), Options = options
            };
            parameters.Add(parameter);
        }

        return [.. parameters];
    }

    public string CalculateText(TemplateParameter[] templateParameters)
    {
        var parameterPattern = @"\{(?<value>[^\}]+)\}";
        return Regex.Replace(Text, parameterPattern, match =>
        {
            var value = match.Groups["value"].Value;
            var parts = value.Split(':');
            var name = parts[0];
            var parameter = templateParameters.FirstOrDefault(p => p.Name == name) ??
                            throw new Exception("Parameter not found");
            return parameter.Value;
        });
    }
}

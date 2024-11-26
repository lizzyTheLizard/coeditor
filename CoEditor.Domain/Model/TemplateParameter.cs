namespace CoEditor.Domain.Model;

public class TemplateParameter
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string Name { get; init; }

    public required TemplateParameterType Type { get; init; }

    public required string[] Options { get; init; }

    public string Value { get; set; } = string.Empty;

    public bool Valid => !string.IsNullOrEmpty(Value);

    public override string ToString() =>
        $"{base.ToString()}: Id={Id}, Name={Name}, Type={Type}, Options={string.Join(",", Options)}, Value={Value}, Valid={Valid}";
}

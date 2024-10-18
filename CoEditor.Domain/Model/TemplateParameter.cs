﻿namespace CoEditor.Domain.Model;

public class TemplateParameter
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required TemplateParameterType Type { get; init; }
    public required string[] Options { get; init; }
    public string Value { get; set; } = "";
    public bool Valid => !string.IsNullOrEmpty(Value);
}
namespace CoEditor.Data;

public class Template
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Text { get; init; }
    public required string UserName { get; init; }
    public required  Language Language { get; init; }
}
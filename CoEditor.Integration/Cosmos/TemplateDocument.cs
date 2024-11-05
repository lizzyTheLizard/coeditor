using CoEditor.Domain.Model;

namespace CoEditor.Integration.Cosmos;

internal class TemplateDocument
{
    public required Guid Id { get; init; }

    public required string UserName { get; init; }

    public required string Name { get; set; }

    public required string Text { get; set; }

    public required Language Language { get; init; }
}

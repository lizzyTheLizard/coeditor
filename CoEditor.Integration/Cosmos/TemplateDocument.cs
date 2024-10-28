using CoEditor.Domain.Model;
using System.Diagnostics.CodeAnalysis;

namespace CoEditor.Integration.Cosmos;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Not relevant for CosmosDB")]
internal class TemplateDocument
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public required Language Language { get; init; }
}

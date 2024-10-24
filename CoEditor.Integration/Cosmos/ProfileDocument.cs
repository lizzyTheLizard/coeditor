using CoEditor.Domain.Model;
using System.Diagnostics.CodeAnalysis;

namespace CoEditor.Integration.Cosmos;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Not relevant for CosmosDB")]
internal class ProfileDocument
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Text { get; init; }
    public required Language Language { get; init; }
}

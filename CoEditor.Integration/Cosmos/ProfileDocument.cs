using CoEditor.Domain.Model;

namespace CoEditor.Integration.Cosmos;

internal class ProfileDocument
{
    public required Guid Id { get; init; }

    public required string UserName { get; init; }

    public required string Text { get; set; }

    public required Language Language { get; init; }
}

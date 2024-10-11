namespace CoEditor.Data;

public class Profile
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public required string UserName { get; init; }
    public required Language Language { get; init; }
}

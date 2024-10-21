using CoEditor.Domain.Model;

namespace CoEditor.Domain.Incomming;

public class HandleActionInput
{
    public required Guid ConversationGuid { get; init; }
    public required string NewContext { get; init; }
    public required string NewText { get; init; }
}

public class HandleNamedActionInput : HandleActionInput
{
    public required ActionName Action { get; init; }
    public required Language Language { get; init; }
    public Selection? Selection { get; init; }
}

public class HandleCustomActionInput : HandleActionInput
{
    public required string Action { get; init; }
}

public class HandleInitialActionInput : HandleActionInput
{
    public required Language Language { get; init; }
}

public record Selection(int Start, int End)
{
}

public enum ActionName { Improve, Expand, Reformulate, Summarize }

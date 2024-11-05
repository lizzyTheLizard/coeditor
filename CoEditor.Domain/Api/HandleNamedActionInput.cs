using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public enum ActionName
{
     Improve,
     Expand,
     Reformulate,
     Summarize,
}

public class HandleNamedActionInput : HandleActionInput
{
    public required ActionName Action { get; init; }

    public required Language Language { get; init; }

    public Selection? Selection { get; init; }
}

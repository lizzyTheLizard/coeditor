using System.Diagnostics.CodeAnalysis;

namespace CoEditor.Domain.Api;

public class HandleCustomActionInput : HandleActionInput
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
        Justification = "This is not used yet but will be when custom command are implemented")]
    public required string Action { get; init; }
}

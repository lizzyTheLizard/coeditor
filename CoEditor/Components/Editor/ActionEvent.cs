using CoEditor.Domain.Api;

namespace CoEditor.Components.Editor;

public record ActionEvent(string? CustomAction, ActionName? ActionName, Selection? Selection);

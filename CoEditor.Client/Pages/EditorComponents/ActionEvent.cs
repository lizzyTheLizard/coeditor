using CoEditor.Domain.Api;

namespace CoEditor.Client.Pages.EditorComponents;

public record ActionEvent(string? CustomAction, ActionName? ActionName, Selection? Selection);

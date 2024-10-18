using CoEditor.Domain.Model;

namespace CoEditor.Domain.Incomming;

public interface IEditorActionApi
{
    Task<string> HandleEditorActionAsync(string userName, EditorActionInput input);
}

public record EditorActionInput(ActionName Action, Language Language, string Context, string CurrentText, Selection? Selection);

public enum ActionName { Improve, Expand, Propose, Reformulate, Summarize }

public record Selection(int Start, int End) { }

public class EditorActionException : Exception { };


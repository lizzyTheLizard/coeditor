using CoEditor.Domain.Model;

namespace CoEditor.Domain.Incomming;

public interface IEditorActionApi
{
    Task<string> InitializeConversationAsync(string userName, InitializeConversationInput input);

    Task<string> HandleEditorCommandAsync(string userName, HandleEditorCommandInput input);

    Task<string> HandleCustomEditorCommandAsync(string userName, HandleCustomEditorCommandInput input);
}

public record InitializeConversationInput(Guid ConversationGuid, Language Language, string Context);

public record HandleEditorCommandInput(Guid ConversationGuid, Language Language, ActionName Action, string Context, string CurrentText, Selection? Selection);

public record HandleCustomEditorCommandInput(Guid ConversationGuid, Language Language, string CustomCommand, string Context, string CurrentText, Selection? Selection);

public enum ActionName { Improve, Expand, Reformulate, Summarize }

public record Selection(int Start, int End) { }

public class EditorActionException(string Message) : Exception(Message) { };

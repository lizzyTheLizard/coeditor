using CoEditor.Logic;

namespace CoEditor.Logic.Actions;

public class CheckAndImprove : IAction
{
    public string Name => "Check and Improve";
    public Task<UndoableTextChange> ApplyAsync(Context context, string Text, Selection? selection)
    {
        //TODO: Implement Command
        throw new NotImplementedException();
    }
}

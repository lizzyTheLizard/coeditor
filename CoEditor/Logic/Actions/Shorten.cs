using CoEditor.Logic;

namespace CoEditor.Logic.Actions;

public class Shorten : IAction
{
    public string Name => "Shorten";
    public Task<UndoableTextChange> ApplyAsync(Context context, string Text, Selection? selection)
    {
        //TODO: Implement Command
        throw new NotImplementedException();
    }
}

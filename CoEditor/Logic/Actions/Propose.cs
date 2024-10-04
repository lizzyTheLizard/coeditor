using CoEditor.Logic;

namespace CoEditor.Logic.Actions;

public class Propose : IAction
{
    public string Name => "Propose";
    public Task<UndoableTextChange> ApplyAsync(Context context, string Text, Selection? selection)
    {
        //TODO: Implement Command
        throw new NotImplementedException();
    }
}
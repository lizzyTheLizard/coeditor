using CoEditor.Logic;

namespace CoEditor.Logic.Actions;

public class Prolong : IAction
{
    public string Name => "Prolong";
    public Task<UndoableTextChange> ApplyAsync(Context context, string Text, Selection? selection)
    {
        //TODO: Implement Command
        throw new NotImplementedException();
    }
}

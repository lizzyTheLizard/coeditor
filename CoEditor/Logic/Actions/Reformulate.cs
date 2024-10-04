using CoEditor.Logic;

namespace CoEditor.Logic.Actions;

public class Reformulate : IAction
{
    public string Name => "Reformulate";
    public Task<UndoableTextChange> ApplyAsync(Context context, string Text, Selection? selection)
    {
        //TODO: Implement Command
        throw new NotImplementedException();
    }
}

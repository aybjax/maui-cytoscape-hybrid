namespace Visualizer8.Services.UndoServiceBase;

public sealed class MultipleUndoRedoCommand : IUndoRedoCommand
{
    private LinkedList<IUndoRedoCommand> undos = new();

    public void AddUndoRedoCommand(IUndoRedoCommand command)
    {
        undos.AddFirst(command);
    }
    public void Execute()
    {
        foreach (var command in undos)
        {
            command.Execute();
        }
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        var result = new MultipleUndoRedoCommand();

        while (undos.Count > 0)
        {
            var command = undos.First().GetRedoCommand();
            undos.RemoveFirst();
            result.AddUndoRedoCommand(command);
        }

        return result;
    }
}

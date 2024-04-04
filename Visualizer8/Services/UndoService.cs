using Visualizer8.Services.UndoServiceBase;

namespace Visualizer8.Services;

public partial class UndoService
{
    private Stack<IUndoRedoCommand> undoCommands  { get; set; }  = new();
    private Stack<IUndoRedoCommand> redoCommands { get; set; } = new();

    public void Clear()
    {
        undoCommands.Clear();
        redoCommands.Clear();
    }

    public void AddUndoAction(IUndoRedoCommand redoCommand)
    {
        redoCommands.Clear();
        undoCommands.Push(redoCommand);
    }

    public void Undo()
    {
        if (undoCommands.Count == 0)
        {
            return;
        }
        
        var action = undoCommands.Pop();
        action.Execute();
        
        var redoAction = action.GetRedoCommand();
        redoCommands.Push(redoAction);
    }

    public void Redo()
    {
        if (redoCommands.Count == 0)
        {
            return;
        }

        var action = redoCommands.Pop();
        action.Execute();

        var redoAction = action.GetRedoCommand();
        undoCommands.Push(redoAction);
    }
}
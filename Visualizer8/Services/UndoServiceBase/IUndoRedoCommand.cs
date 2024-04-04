namespace Visualizer8.Services.UndoServiceBase;


public interface IUndoRedoCommand
{
    void Execute();
    IUndoRedoCommand GetRedoCommand();
}
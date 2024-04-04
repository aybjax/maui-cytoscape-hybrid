using Visualizer8.Models.GraphData;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class EdgeDeletionUndoRedoCommand(Edge deletedEdge) : IUndoRedoCommand
{

    public void Execute()
    {
        UndoService.EdgeDeletionUndoEvent?.Invoke(this, new ()
        {
            CreatingEdge = deletedEdge,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new EdgeCreationUndoRedoCommand(deletedEdge);
    }
}
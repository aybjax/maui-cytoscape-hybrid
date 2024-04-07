using Visualizer8.Models.GraphData;
using Visualizer8.Services.UndoServiceBase.Type;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class EdgeDeletionUndoRedoCommand(Edge deletedEdge, EdgeType edgeType) : IUndoRedoCommand
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
        return new EdgeCreationUndoRedoCommand(deletedEdge, edgeType);
    }
}
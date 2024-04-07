using Visualizer8.Models.GraphData;
using Visualizer8.Services.UndoServiceBase.Type;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class EdgeCreationUndoRedoCommand(Edge createdEdge, EdgeType edgeType) : IUndoRedoCommand
{
    public void Execute()
    {
        UndoService.EdgeCreationUndoEvent?.Invoke(this, new ()
        {
            DeletingId = createdEdge.Id,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new EdgeDeletionUndoRedoCommand(createdEdge, edgeType);
    }
}
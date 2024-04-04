using Visualizer8.Models.GraphData;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class EdgeCreationUndoRedoCommand(Edge createdEdge) : IUndoRedoCommand
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
        return new EdgeDeletionUndoRedoCommand(createdEdge);
    }
}
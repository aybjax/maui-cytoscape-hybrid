using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class PositionUpdateUndoCommand(GraphId microtopicId, Position newPosition, Position oldPosition): IUndoRedoCommand
{
    public void Execute()
    {
        UndoService.PositionUpdateUndoEvent?.Invoke(this, new ()
        {
            MicrotopicId = microtopicId,
            Position = oldPosition,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new PositionUpdateUndoCommand(microtopicId, oldPosition, newPosition);
    }
}
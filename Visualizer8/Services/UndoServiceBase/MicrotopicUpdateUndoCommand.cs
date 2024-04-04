using Visualizer8.Models.GraphData;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class MicrotopicUpdateUndoCommand(Node newMicrotopic, Node oldMicrotopic): IUndoRedoCommand
{
    public void Execute()
    {
        UndoService.MicrotopicUpdateUndoEvent?.Invoke(this, new ()
        {
            DeletingId = newMicrotopic.Id,
            ReplacementMicrotopic = oldMicrotopic,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new MicrotopicUpdateUndoCommand(oldMicrotopic, newMicrotopic);
    }
}
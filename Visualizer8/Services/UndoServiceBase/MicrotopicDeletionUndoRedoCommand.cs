using Visualizer8.Models.GraphData;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class MicrotopicDeletionUndoRedoCommand(MicrotopicNode deletedMicrotopic) : IUndoRedoCommand
{
    public void Execute()
    {
        UndoService.MicrotopicDeletionUndoEvent?.Invoke(this, new ()
        {
            CreatingMicrotopic = deletedMicrotopic,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new MicrotopicCreationUndoRedoCommand(deletedMicrotopic);
    }
}
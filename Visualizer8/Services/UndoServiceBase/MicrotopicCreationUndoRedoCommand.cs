using Visualizer8.Models.GraphData;

namespace Visualizer8.Services.UndoServiceBase;

public sealed class MicrotopicCreationUndoRedoCommand(Node createdMicrotopic) : IUndoRedoCommand
{

    public void Execute()
    {
        UndoService.MicrotopicCreationUndoEvent?.Invoke(this, new ()
        {
            DeletingId = createdMicrotopic.Id,
        });
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        return new MicrotopicDeletionUndoRedoCommand(createdMicrotopic);
    }
}
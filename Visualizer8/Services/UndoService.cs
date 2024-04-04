using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public class UndoService
{
    public sealed class MicrotopicCreationUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
    }
    public static EventHandler<MicrotopicCreationUndoEventArg>? MicrotopicCreationUndoEvent;
    
    public sealed class MicrotopicDeletionUndoEventArg: EventArgs
    {
        public required Node CreatingMicrotopic { get; init; }
    }

    public static EventHandler<MicrotopicDeletionUndoEventArg>? MicrotopicDeletionUndoEvent;
    
    public sealed class EdgeCreationUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
    }

    public static EventHandler<EdgeCreationUndoEventArg>? EdgeCreationUndoEvent;
    
    public sealed class EdgeDeletionUndoEventArg: EventArgs
    {
        public required Edge CreatingEdge { get; init; }
    }

    public static EventHandler<EdgeDeletionUndoEventArg>? EdgeDeletionUndoEvent;
    
    public sealed class MicrotopicUpdateUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
        public required Node ReplacementMicrotopic { get; init; }
    }

    public static EventHandler<MicrotopicUpdateUndoEventArg>? MicrotopicUpdateUndoEvent;
    
    public sealed class PositionUpdateUndoEventArg: EventArgs
    {
        public required GraphId MicrotopicId { get; init; }
        public required Position Position { get; init; }
    }

    public static EventHandler<PositionUpdateUndoEventArg>? PositionUpdateUndoEvent;
    
    //
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

public interface IUndoRedoCommand
{
    void Execute();
    IUndoRedoCommand GetRedoCommand();
}

public sealed class MultipleUndoRedoCommand : IUndoRedoCommand
{
    private LinkedList<IUndoRedoCommand> undos = new();

    public void AddUndoRedoCommand(IUndoRedoCommand command)
    {
        undos.AddFirst(command);
    }
    public void Execute()
    {
        foreach (var command in undos)
        {
            command.Execute();
        }
    }

    public IUndoRedoCommand GetRedoCommand()
    {
        var result = new MultipleUndoRedoCommand();

        while (undos.Count > 0)
        {
            var command = undos.First().GetRedoCommand();
            undos.RemoveFirst();
            result.AddUndoRedoCommand(command);
        }

        return result;
    }
}

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

public sealed class MicrotopicDeletionUndoRedoCommand(Node deletedMicrotopic) : IUndoRedoCommand
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

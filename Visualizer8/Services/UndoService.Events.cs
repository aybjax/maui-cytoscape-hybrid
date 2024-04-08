using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public partial class UndoService
{
    public static EventHandler<MicrotopicCreationUndoEventArg>? MicrotopicCreationUndoEvent;
    
    public static EventHandler<MicrotopicDeletionUndoEventArg>? MicrotopicDeletionUndoEvent;

    public static EventHandler<EdgeCreationUndoEventArg>? EdgeCreationUndoEvent;

    public static EventHandler<EdgeDeletionUndoEventArg>? EdgeDeletionUndoEvent;

    public static EventHandler<MicrotopicUpdateUndoEventArg>? MicrotopicUpdateUndoEvent;

    public static EventHandler<PositionUpdateUndoEventArg>? PositionUpdateUndoEvent;

    //
    
    public sealed class MicrotopicCreationUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
    }

    public sealed class MicrotopicDeletionUndoEventArg: EventArgs
    {
        public required MicrotopicNode CreatingMicrotopic { get; init; }
    }

    public sealed class EdgeCreationUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
    }

    public sealed class EdgeDeletionUndoEventArg: EventArgs
    {
        public required Edge CreatingEdge { get; init; }
    }

    public sealed class MicrotopicUpdateUndoEventArg: EventArgs
    {
        public required GraphId DeletingId { get; init; }
        public required Node ReplacementMicrotopic { get; init; }
    }

    public sealed class PositionUpdateUndoEventArg: EventArgs
    {
        public required GraphId MicrotopicId { get; init; }
        public required Position Position { get; init; }
    }
}
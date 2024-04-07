using Visualizer8.Extensions.Types;
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public partial class JsService
{
    public event AsyncEventHandler<OnMicrotopicAdditionArg>? OnMicrotopicAdditionRequestedEvent;
    public event AsyncEventHandler<OnMicrotopicUpdatedArg>? OnMicrotopicUpdateRequestedEvent;
    public event EventHandler<OnMicrotopicDeletionArg>? OnMicrotopicDeletionRequestedEvent;
    public event EventHandler<OnEdgeAdditionArg>? OnEdgeAdditionRequestedEvent;
    public event EventHandler<OnEdgeDeletionArg>? OnEdgeDeletionRequestedEvent;
    public event EventHandler<OnPositionUpdatedArg>? OnNodePositionUpdatedEvent;
    public event EventHandler<OnPositionUpdatedArg>? OnNodePositionInitiatedEvent;
    public event EventHandler<AddUnitRequestedArg>? AddUnitRequestedEvent;
    public event EventHandler<DeleteUnitRequestedArg>? DeleteUnitRequestedEvent;
    //
    public class AddUnitRequestedArg : EventArgs
    {
        public required GraphId MicrotopicId { get; init; }
    }
    public class DeleteUnitRequestedArg : EventArgs
    {
        public required GraphId UnitId { get; init; }
    }
    public class OnMicrotopicAdditionArg: EventArgs{}

    public class OnMicrotopicUpdatedArg : EventArgs
    {
        public required GraphId Id { get; init; }
    }
    public class OnMicrotopicDeletionArg: EventArgs
    {
        public required GraphId Id { get; init; }
    }
    public class OnEdgeAdditionArg: EventArgs
    {
        public required GraphSource SourceId { get; init; }
        public required GraphTarget TargetId { get; init; }
    }
    public class OnEdgeDeletionArg: EventArgs
    {
        public required GraphId Id { get; init; }
    }
    public class OnPositionUpdatedArg: EventArgs
    {
        public required GraphId Id { get; init; }
        public required Position Position {get; init; }
    }
}
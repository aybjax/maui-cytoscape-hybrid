using Visualizer8.Extensions.Types;
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public partial class GraphService
{
    
    public event EventHandler? OnDataInitialized;

    public event AsyncEventHandler<OnMicrotopicAdditionEvaluatedArg>? OnMicrotopicAdditionEvaluatedEvent;
    public event AsyncEventHandler<OnMicrotopicWithPositionAdditionEvaluatedArg>? OnMicrotopicWithPositionAdditionEvaluatedEvent;

    public event AsyncEventHandler<OnMicrotopicUpdateEvaluatedArg>? OnMicrotopicUpdateEvaluatedEvent;
    
    public event AsyncEventHandler<OnMicrotopicDeletionEvaluatedArg>? OnMicrotopicDeletionEvaluatedEvent;
    
    public event AsyncEventHandler<OnEdgeAdditionEvaluatedArg>? OnEdgeAdditionEvaluatedEvent;
    
    public event AsyncEventHandler<OnEdgeDeletionEvaluatedArg>? OnEdgeDeletionEvaluatedEvent;
    
    public event AsyncEventHandler<OnNodePositionEvaluatedArg>? OnNodePositionEvaluatedEvent;
    
    public class OnMicrotopicAdditionEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }
        public required GraphName? Name { get; init; }
        public required GraphName? ParentName { get; init; }
        public required string? Color { get; init; }

        public static OnMicrotopicAdditionEvaluatedArg Empty => new()
        {
            Id = null,
            Name = null,
            ParentName = null,
            Color = null,
        };
    }

    public class OnMicrotopicWithPositionAdditionEvaluatedArg : OnMicrotopicAdditionEvaluatedArg
    {
        public required Position? Position { get; init; }

        public static OnMicrotopicWithPositionAdditionEvaluatedArg Empty => new()
        {
            Id = null,
            Name = null,
            ParentName = null,
            Color = null,
            Position = null,
        };
    }

    public class OnMicrotopicUpdateEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }
        public required GraphName? Name { get; init; }
        public required GraphName? ParentName { get; init; }
        public required string? Color { get; init; }

        public static OnMicrotopicUpdateEvaluatedArg Empty => new()
        {
            Id = null,
            Name = null,
            ParentName = null,
            Color = null,
        };
    }
    
    public class OnMicrotopicDeletionEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }
        public required GraphId[]? EdgeIds { get; init; }

        public static OnMicrotopicDeletionEvaluatedArg Empty => new()
        {
            Id = null,
            EdgeIds = null,
        };
    }


    public class OnEdgeAdditionEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }
        public required GraphSource? SourceId { get; init; }
        public required GraphTarget? TargetId { get; init; }

        public static OnEdgeAdditionEvaluatedArg Empty => new()
        {
            Id = null,
            SourceId = null,
            TargetId = null,
        };
    }


    public class OnEdgeDeletionEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }

        public static OnEdgeDeletionEvaluatedArg Empty => new()
        {
            Id = null,
        };
    }

    public class OnNodePositionEvaluatedArg: EventArgs
    {
        public required GraphId? Id { get; init; }
        public required Position? Position { get; init; }

        public static OnNodePositionEvaluatedArg Empty => new()
        {
            Id = null,
            Position = null,
        };
    }
}
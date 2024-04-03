using Microsoft.JSInterop;
using Visualizer8.Extensions.Types;
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public class JsService
{
    private readonly GraphService _graphService;

    public JsService(GraphService graphService, IJSRuntime Js)
    {
        _graphService = graphService;
        OnMicrotopicAdditionRequestedEvent += _graphService.OnMicrotopicAdded;
        OnMicrotopicUpdateRequestedEvent += _graphService.OnMicrotopicUpdated;
        OnMicrotopicDeletionRequestedEvent += _graphService.OnMicrotopicDeleted;
        OnEdgeAdditionRequestedEvent += _graphService.OnEdgeAdded;
        OnEdgeDeletionRequestedEvent += _graphService.OnEdgeDeleted;
        OnNodePositionUpdatedEvent += _graphService.OnNodePositionUpdated;
    }

    [JSInvokable(nameof(AddMicrotopic))]
    public void AddMicrotopic()
    {
        OnMicrotopicAdditionRequestedEvent?.Invoke(this, new());
    }

    [JSInvokable(nameof(UpdateMicrotopicById))]
    public void UpdateMicrotopicById(string id)
    {
        OnMicrotopicUpdateRequestedEvent?.Invoke(this, new()
        {
            Id = id,
        });
    }

    [JSInvokable(nameof(DeleteMicrotopicById))]
    public void DeleteMicrotopicById(string id)
    {
        OnMicrotopicDeletionRequestedEvent?.Invoke(this, new()
        {
            Id = id,
        });
    }

    [JSInvokable(nameof(AddEdgeBySourceTarget))]
    public void AddEdgeBySourceTarget(string sourceId, string targetId)
    {
        OnEdgeAdditionRequestedEvent?.Invoke(this, new()
        {
            SourceId = sourceId,
            TargetId = targetId,
        });
    }

    [JSInvokable(nameof(DeleteEdgeById))]
    public void DeleteEdgeById(string id)
    {
        OnEdgeDeletionRequestedEvent?.Invoke(this, new()
        {
            Id = id,
        });
    }

    [JSInvokable(nameof(UpdateNodePositionById))]
    public void UpdateNodePositionById(string id, float x, float y)
    {
        OnNodePositionUpdatedEvent?.Invoke(this, new()
        {
            Id = id,
            Position = new (x, y),
        });
    }
    
    //
    
    public event AsyncEventHandler<OnMicrotopicAdditionArg>? OnMicrotopicAdditionRequestedEvent;
    public event AsyncEventHandler<OnMicrotopicUpdatedArg>? OnMicrotopicUpdateRequestedEvent;
    public event EventHandler<OnMicrotopicDeletionArg>? OnMicrotopicDeletionRequestedEvent;
    public event EventHandler<OnEdgeAdditionArg>? OnEdgeAdditionRequestedEvent;
    public event EventHandler<OnEdgeDeletionArg>? OnEdgeDeletionRequestedEvent;
    public event EventHandler<OnPositionUpdatedArg>? OnNodePositionUpdatedEvent;
    //
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
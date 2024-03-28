using Microsoft.JSInterop;
using Visualizer8.Extensions.Types;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public class JsService
{
    private readonly GraphService _graphService;

    public JsService(GraphService graphService, IJSRuntime Js)
    {
        _graphService = graphService;
        OnMicrotopicAdditionRequestedEvent += _graphService.OnMicrotopicAdded;
        OnMicrotopicDeletionRequestedEvent += _graphService.OnMicrotopicDeleted;
        OnEdgeAdditionRequestedEvent += _graphService.OnEdgeAdded;
        OnEdgeDeletionRequestedEvent += _graphService.OnEdgeDeleted;
    }

    [JSInvokable(nameof(AddMicrotopic))]
    public void AddMicrotopic()
    {
        OnMicrotopicAdditionRequestedEvent?.Invoke(this, new());
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
    
    //
    
    public event AsyncEventHandler<OnMicrotopicAdditionArg>? OnMicrotopicAdditionRequestedEvent;
    public event EventHandler<OnMicrotopicDeletionArg>? OnMicrotopicDeletionRequestedEvent;
    public event EventHandler<OnEdgeAdditionArg>? OnEdgeAdditionRequestedEvent;
    public event EventHandler<OnEdgeDeletionArg>? OnEdgeDeletionRequestedEvent;
    //
    public class OnMicrotopicAdditionArg: EventArgs{}
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
}
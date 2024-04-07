using Microsoft.JSInterop;
using Visualizer8.Extensions.Types;
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Services;

public partial class JsService
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
        AddUnitRequestedEvent += _graphService.OnUnitAdded;
        DeleteUnitRequestedEvent += _graphService.OnUnitDeleted;
        OnNodePositionUpdatedEvent += _graphService.OnNodePositionUpdated;
        OnNodePositionInitiatedEvent += _graphService.OnNodePositionInitiated;
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

    [JSInvokable(nameof(InitiateNodePositionById))]
    public void InitiateNodePositionById(string id, float x, float y)
    {
        OnNodePositionInitiatedEvent?.Invoke(this, new()
        {
            Id = id,
            Position = new (x, y),
        });
    }

    [JSInvokable(nameof(AddUnit))]
    public void AddUnit(string id)
    {
        AddUnitRequestedEvent?.Invoke(this, new()
        {
            MicrotopicId = id,
        });
    }

    [JSInvokable(nameof(DeleteUnit))]
    public void DeleteUnit(string id)
    {
        DeleteUnitRequestedEvent?.Invoke(this, new()
        {
            UnitId = id,
        });
    }
}
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;
using Visualizer8.Services.UndoServiceBase;

namespace Visualizer8.Services;

public partial class GraphService
{
    public async Task OnMicrotopicAdded(object? sender, JsService.OnMicrotopicAdditionArg arg)
    {
        var newMicrotopic = await GetTopicId();

        if (newMicrotopic is null)
        {
            return;
        }

        var parent = Raw.Topics.FirstOrDefault(t => t.Id == newMicrotopic.ParentId);

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.ParentId, newMicrotopic.Position);
        _raw.Microtopics.Add(microtopic);
        
        if (!_mcColor.TryGetValue(microtopic.Parent ?? "none", out string? color))
        {
            color = _mcColorSpare.FirstOrDefault() ?? "#000";
            _mcColorSpare.Remove(color);
            _mcColor[microtopic.Parent?? "none"] = color;
        }
        
        OnMicrotopicAdditionEvaluatedEvent?.Invoke(this, new OnMicrotopicAdditionEvaluatedArg()
        {
            Id = microtopic.Id,
            Name = microtopic.Name,
            ParentName = parent?.Name,
            Color = color,
        });
        _undoService.AddUndoAction(new MicrotopicCreationUndoRedoCommand(microtopic));
    }
    public void OnMicrotopicDeletionUndo(object? sender, UndoService.MicrotopicDeletionUndoEventArg arg)
    {
        var newMicrotopic = arg.CreatingMicrotopic;

        var parent = Raw.Topics.FirstOrDefault(t => t.Id == newMicrotopic.Parent);

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.Parent, newMicrotopic.Position);
        _raw.Microtopics.Add(microtopic);
        
        if (!_mcColor.TryGetValue(microtopic.Parent ?? "none", out string? color))
        {
            color = _mcColorSpare.FirstOrDefault() ?? "#000";
            _mcColorSpare.Remove(color);
            _mcColor[microtopic.Parent?? "none"] = color;
        }
        
        OnMicrotopicWithPositionAdditionEvaluatedEvent?.Invoke(this, new()
        {
            Id = microtopic.Id,
            Name = microtopic.Name,
            ParentName = parent?.Name,
            Color = color,
            Position = microtopic.Position,
        }); 
    }

    public async Task OnMicrotopicUpdated(object? sender, JsService.OnMicrotopicUpdatedArg arg)
    {
        var newMicrotopic = await GetTopicId(arg.Id);

        if (newMicrotopic is null)
        {
            return;
        }

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.ParentId, newMicrotopic.Position);
        var oldMicrotopic = _raw.Microtopics.First(m => m.Id == newMicrotopic.Id);
        
        _raw.Microtopics.RemoveWhere(m => m.Id == newMicrotopic.Id); // TODO test
        _raw.Microtopics.Add(microtopic);
        
        var newParent = Raw.Topics.FirstOrDefault(t => t.Id == microtopic.Parent);
        
        if (!_mcColor.TryGetValue(microtopic.Parent ?? "none", out string? color))
        {
            color = _mcColorSpare.FirstOrDefault() ?? "#000";
            _mcColorSpare.Remove(color);
            _mcColor[microtopic.Parent?? "none"] = color;
        }
        
        OnMicrotopicUpdateEvaluatedEvent?.Invoke(this, new OnMicrotopicUpdateEvaluatedArg()
        {
            Id = microtopic.Id,
            Name = microtopic.Name,
            ParentName = newParent?.Name,
            Color = color,
        });
        _undoService.AddUndoAction(new MicrotopicUpdateUndoCommand(microtopic, oldMicrotopic));
    }
    public void OnMicrotopicUpdatedUndo(object? sender, UndoService.MicrotopicUpdateUndoEventArg arg)
    {
        var newMicrotopic = arg.ReplacementMicrotopic;

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.Parent, newMicrotopic.Position);
        var oldMicrotopic = _raw.Microtopics.First(m => m.Id == arg.DeletingId);
        
        _raw.Microtopics.RemoveWhere(m => m.Id == newMicrotopic.Id); // TODO test
        _raw.Microtopics.Add(microtopic);

        var newParent = Raw.Topics.FirstOrDefault(t => t.Id == microtopic.Parent);
        
        if (!_mcColor.TryGetValue(microtopic.Parent ?? "none", out string? color))
        {
            color = _mcColorSpare.FirstOrDefault() ?? "#000";
            _mcColorSpare.Remove(color);
            _mcColor[microtopic.Parent?? "none"] = color;
        }

        OnMicrotopicUpdateEvaluatedEvent?.Invoke(this, new OnMicrotopicUpdateEvaluatedArg()
        {
            Id = microtopic.Id,
            Name = microtopic.Name,
            ParentName = newParent?.Name,
            Color = color,
        });
    }
    
    public void OnMicrotopicDeleted(object? sender, JsService.OnMicrotopicDeletionArg arg)
    {
        var edges = new List<GraphId>(Raw.Edges.Count);
        Node? microtopicForUndo = default;
        var edgesForUndo = new List<Edge>(Raw.Edges.Count);

        var nbr = Raw.Microtopics.RemoveWhere(n =>
        {
            if (n.Id == arg.Id)
            {
                microtopicForUndo = n;
                
                return true;
            }

            return false;
        });

        if (nbr > 0)
        {
            var removedInt = Raw.Edges.RemoveWhere(e =>
            {
                if (e.Source == arg.Id || e.Target == arg.Id)
                {
                    edges.Add(e.Id);
                    edgesForUndo.Add(e);
                    return true;
                }

                return false;
            });
            
            OnMicrotopicDeletionEvaluatedEvent?.Invoke(this, new OnMicrotopicDeletionEvaluatedArg()
            {
                Id = arg.Id,
                EdgeIds = edges.ToArray(),
            });

            var undos = new MultipleUndoRedoCommand();
            
            foreach (var edge in edgesForUndo)
            {
                undos.AddUndoRedoCommand(new EdgeDeletionUndoRedoCommand(edge));
            }
            if (microtopicForUndo is not null)
            {
                undos.AddUndoRedoCommand(new MicrotopicDeletionUndoRedoCommand(microtopicForUndo));
            }
            _undoService.AddUndoAction(undos);
        }
    }
    public void OnMicrotopicCreationUndo(object? sender, UndoService.MicrotopicCreationUndoEventArg arg)
    {
        var nbr = Raw.Microtopics.RemoveWhere(n => n.Id == arg.DeletingId);

        if (nbr > 0)
        {
            OnMicrotopicDeletionEvaluatedEvent?.Invoke(this, new OnMicrotopicDeletionEvaluatedArg()
            {
                Id = arg.DeletingId,
                EdgeIds = Array.Empty<GraphId>(),
            });
        }
    }
    
    public void OnEdgeAdded(object? sender, JsService.OnEdgeAdditionArg arg)
    {
        var source = Raw.Microtopics.FirstOrDefault(m => m.Id == arg.SourceId);
        if (source is null)
        {
            return;    
        }
        
        var target = Raw.Microtopics.FirstOrDefault(m => m.Id == arg.TargetId);
        if (target is null)
        {
            return;    
        }

        var edge = Raw.Edges.FirstOrDefault(e =>
            (e.Source == arg.SourceId && e.Target == arg.TargetId)
            || (e.Source == arg.TargetId && e.Target == arg.SourceId));

        if (edge is not null)
        {
            return;
        }

        edge = new Edge(Guid.NewGuid(), arg.SourceId, arg.TargetId);
        _raw.Edges.Add(edge);

        OnEdgeAdditionEvaluatedEvent?.Invoke(this, new OnEdgeAdditionEvaluatedArg()
        {
            Id = edge.Id,
            SourceId = edge.Source,
            TargetId = edge.Target,
        });
        
        _undoService.AddUndoAction(new EdgeCreationUndoRedoCommand(edge));
    }

    public void OnEdgeDeletionUndo(object? sender, UndoService.EdgeDeletionUndoEventArg arg)
    {
        var source = Raw.Microtopics.FirstOrDefault(m => m.Id == arg.CreatingEdge.Source);
        if (source is null)
        {
            return;    
        }
        
        var target = Raw.Microtopics.FirstOrDefault(m => m.Id == arg.CreatingEdge.Target);
        if (target is null)
        {
            return;    
        }

        var edge = Raw.Edges.FirstOrDefault(e =>
            (e.Source == arg.CreatingEdge.Source && e.Target == arg.CreatingEdge.Target)
            || (e.Source == arg.CreatingEdge.Target && e.Target == arg.CreatingEdge.Source));
        if (edge is not null)
        {
            return;
        }

        edge = arg.CreatingEdge with {};
        _raw.Edges.Add(edge);

        OnEdgeAdditionEvaluatedEvent?.Invoke(this, new OnEdgeAdditionEvaluatedArg()
        {
            Id = edge.Id,
            SourceId = edge.Source,
            TargetId = edge.Target,
        });
    }
    
    public void OnEdgeDeleted(object? sender, JsService.OnEdgeDeletionArg arg)
    {
        Edge? edgeForUndo = default;
        var edgeNbr = _raw.Edges.RemoveWhere(e =>
        {
            if (e.Id == arg.Id)
            {
                edgeForUndo = e;
                return true;
            }

            return false;
        });

        if (edgeNbr == 0)
        {
            return;
        }
        
        OnEdgeDeletionEvaluatedEvent?.Invoke(this, new OnEdgeDeletionEvaluatedArg()
        {
            Id = arg.Id,
        });

        if (edgeForUndo is not null)
        {
            _undoService.AddUndoAction(new EdgeDeletionUndoRedoCommand(edgeForUndo));
        }
    }
    public void OnEdgeCreationUndo(object? sender, UndoService.EdgeCreationUndoEventArg arg)
    {
        var edgeNbr = _raw.Edges.RemoveWhere(e => e.Id == arg.DeletingId);

        if (edgeNbr == 0)
        {
            return;
        }

        OnEdgeDeletionEvaluatedEvent?.Invoke(this, new OnEdgeDeletionEvaluatedArg()
        {
            Id = arg.DeletingId,
        });
    }
    
    public void OnNodePositionUpdated(object? sender, JsService.OnPositionUpdatedArg arg)
    {
        var microtopic = _raw.Microtopics.First(m => m.Id == arg.Id);
        var newMicrotopic = new Node(microtopic.Id, microtopic.Name, microtopic.Parent, arg.Position);

        _raw.Microtopics.RemoveWhere(m => m.Id == microtopic.Id);
        _raw.Microtopics.Add(newMicrotopic);

        _undoService.AddUndoAction(new PositionUpdateUndoCommand(microtopic.Id, arg.Position, microtopic.Position ?? new Position(0, 0)));
    }
    
    public void OnNodePositionInitiated(object? sender, JsService.OnPositionUpdatedArg arg)
    {
        var microtopic = _raw.Microtopics.First(m => m.Id == arg.Id);
        var newMicrotopic = new Node(microtopic.Id, microtopic.Name, microtopic.Parent, arg.Position);

        _raw.Microtopics.RemoveWhere(m => m.Id == microtopic.Id);
        _raw.Microtopics.Add(newMicrotopic);
    }
    
    public void OnNodePositionUpdateUndo(object? sender, UndoService.PositionUpdateUndoEventArg arg)
    {
        var microtopic = _raw.Microtopics.First(m => m.Id == arg.MicrotopicId);
        var newMicrotopic = microtopic with
        {
            Position = arg.Position,
        };

        _raw.Microtopics.RemoveWhere(m => m.Id == microtopic.Id);
        _raw.Microtopics.Add(newMicrotopic);

        OnNodePositionEvaluatedEvent?.Invoke(this, new()
        {
            Id = newMicrotopic.Id,
            Position = newMicrotopic.Position,
        });
    }
}
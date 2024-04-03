using System.Text.Json;
using Mopups.Services;
using Visualizer8.Extensions;
using Visualizer8.Extensions.Types;
using Visualizer8.Models;
using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;
using Visualizer8.Models.Input;
using Visualizer8.Popups;

namespace Visualizer8.Services;

public class GraphService
{
    Dictionary<string, string> _mcColor = new();
    private HashSet<string> _mcColorSpare = new();

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

    private readonly IServiceProvider _serviceProvider;
    
    public event EventHandler? OnDataInitialized;

    public event AsyncEventHandler<OnMicrotopicAdditionEvaluatedArg>? OnMicrotopicAdditionEvaluatedEvent;

    public event AsyncEventHandler<OnMicrotopicUpdateEvaluatedArg>? OnMicrotopicUpdateEvaluatedEvent;
    
    public event AsyncEventHandler<OnMicrotopicDeletionEvaluatedArg>? OnMicrotopicDeletionEvaluatedEvent;
    
    public event AsyncEventHandler<OnEdgeAdditionEvaluatedArg>? OnEdgeAdditionEvaluatedEvent;
    
    public event AsyncEventHandler<OnEdgeDeletionEvaluatedArg>? OnEdgeDeletionEvaluatedEvent;

    public async Task OnMicrotopicAdded(object? sender, JsService.OnMicrotopicAdditionArg arg)
    {
        var newMicrotopic = await GetTopicId();

        if (newMicrotopic is null)
        {
            return;
        }

        var parent = Unit.UnitTree.SelectMany(u => u.Topics)
            .FirstOrDefault(t => t.Value.Id == newMicrotopic.ParentId);
        
        if (parent is null)
        {
            parent = Unit.SpareTopics.FirstOrDefault(t => t.Value.Id == newMicrotopic.ParentId);
        }

        if (parent is null)
        {
            return;
        }

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.ParentId);
        parent.Microtopics.Add(microtopic);
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
            ParentName = parent.Value.Name,
            Color = color,
        });
    }

    public async Task OnMicrotopicUpdated(object? sender, JsService.OnMicrotopicUpdatedArg arg)
    {
        var newMicrotopic = await GetTopicId(arg.Id);

        if (newMicrotopic is null)
        {
            return;
        }

        var microtopic = new Node(newMicrotopic.Id, newMicrotopic.Name, newMicrotopic.ParentId);
        var oldMicrotopic = _raw.Microtopics.First(m => m.Id == newMicrotopic.Id);
        
        _raw.Microtopics.RemoveWhere(m => m.Id == newMicrotopic.Id); // TODO test
        _raw.Microtopics.Add(microtopic);

        var oldParent = Unit.UnitTree.SelectMany(u => u.Topics)
            .FirstOrDefault(t => t.Value.Id == oldMicrotopic.Parent);
        if (oldParent is null)
        {
            oldParent = Unit.SpareTopics.FirstOrDefault(t => t.Value.Id == oldMicrotopic.Parent);
        }
        oldParent?.Microtopics.RemoveWhere(m => m.Id == oldMicrotopic.Id);

        var newParent = Unit.UnitTree.SelectMany(u => u.Topics)
            .FirstOrDefault(t => t.Value.Id == microtopic.Parent);
        if (newParent is null)
        {
            newParent = Unit.SpareTopics.FirstOrDefault(t => t.Value.Id == microtopic.Parent);
        }
        newParent?.Microtopics.Add(microtopic);
        
        
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
            ParentName = newParent.Value.Name,
            Color = color,
        });
    }
    
    public void OnMicrotopicDeleted(object? sender, JsService.OnMicrotopicDeletionArg arg)
    {
        var edges = new List<GraphId>(Unit.Relations.Count);
        var topics = Unit.UnitTree.SelectMany(u => u.Topics).ToList();
        topics.AddRange(Unit.SpareTopics);

        foreach (var topic in topics)
        {
            var nbr = topic.Microtopics.RemoveWhere(n => n.Id == arg.Id);

            if (nbr > 0)
            {
                if (_raw.Microtopics.RemoveWhere(n => n.Id == arg.Id) == 0)
                {
                    throw new Exception("microtopic not deleted");
                }
                
                var removedInt = Unit.Relations.RemoveWhere(e =>
                {
                    if (e.Source == arg.Id || e.Target == arg.Id)
                    {
                        edges.Add(e.Id);
                        return true;
                    }

                    return false;
                });
                Console.WriteLine($"{removedInt} edges deleted when deleting {arg.Id}");
                _raw.Edges.RemoveWhere(e =>
                {
                    if (e.Source == arg.Id || e.Target == arg.Id)
                    {
                        edges.Add(e.Id);
                        return true;
                    }

                    return false;
                });
                
                OnMicrotopicDeletionEvaluatedEvent?.Invoke(this, new OnMicrotopicDeletionEvaluatedArg()
                {
                    Id = arg.Id,
                    EdgeIds = edges.ToArray(),
                });

                return;
            }
        }
    }
    
    public void OnEdgeAdded(object? sender, JsService.OnEdgeAdditionArg arg)
    {
        var source = Unit.UnitTree.SelectMany(u => u.Topics)
            .SelectMany(t => t.Microtopics).FirstOrDefault(m => m.Id == arg.SourceId);
        
        if (source is null)
        {
            source = Unit.SpareMicrotopics.FirstOrDefault(m => m.Id == arg.SourceId);
        }

        if (source is null)
        {
            return;    
        }
        
        var target = Unit.UnitTree.SelectMany(u => u.Topics)
            .SelectMany(t => t.Microtopics).FirstOrDefault(m => m.Id == arg.TargetId);
        
        if (target is null)
        {
            target = Unit.SpareMicrotopics.FirstOrDefault(m => m.Id == arg.TargetId);
        }

        if (target is null)
        {
            return;    
        }

        var edge = Unit.Relations.FirstOrDefault(e =>
            (e.Source == arg.SourceId && e.Target == arg.TargetId)
            || (e.Source == arg.TargetId && e.Target == arg.SourceId));

        if (edge is not null)
        {
            return;
        }

        edge = new Edge(Guid.NewGuid(), arg.SourceId, arg.TargetId);
        Unit.Relations.Add(edge);
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
        var edgeNbr = Unit.Relations.RemoveWhere(e => e.Id == arg.Id);

        if (edgeNbr == 0)
        {
            return;
        }
        _raw.Edges.RemoveWhere(e => e.Id == arg.Id);
        
        OnEdgeDeletionEvaluatedEvent?.Invoke(this, new OnEdgeDeletionEvaluatedArg()
        {
            Id = arg.Id,
        });
    }
    
    public void OnNodePositionUpdated(object? sender, JsService.OnPositionUpdatedArg arg)
    {
        var microtopic = _raw.Microtopics.First(m => m.Id == arg.Id);
        var newMicrotopic = new Node(microtopic.Id, microtopic.Name, microtopic.Parent, arg.Position);

        _raw.Microtopics.RemoveWhere(m => m.Id == microtopic.Id);
        _raw.Microtopics.Add(newMicrotopic);
        //
        if (microtopic.Parent is null) return;

        var parent = Unit.UnitTree.SelectMany(u => u.Topics)
            .First(t => t.Value.Id == microtopic.Parent);
        if (parent is null)
        {
            parent = Unit.SpareTopics.First(t => t.Value.Id == microtopic.Parent);
        }

        parent.Microtopics.RemoveWhere(m => m.Id == microtopic.Id);
        parent.Microtopics.Add(newMicrotopic);
    }
    
    private GraphDataRaw _raw = new()
    {
        Units = new HashSet<Node>(),
        Topics = new HashSet<Node>(),
        Microtopics = new HashSet<Node>(),
        Edges = new HashSet<Edge>()
    };
    private Graph _unit = new()
    {
        UnitTree = new HashSet<Unit>(),
        SpareTopics = new HashSet<Topic>(),
        SpareMicrotopics = new HashSet<Node>(),
        Relations = new HashSet<Edge>()
    };

    public GraphService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public GraphDataRaw Raw => _raw;
    public Graph Unit => _unit;

    public GraphService InitializeData(string data)
    {
        _mcColorSpare = _spareColors.Select(e => e).ToHashSet();
        _mcColor = new();
        
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters  = {
                new GraphIdJsonConverter(),
                new GraphNameJsonConverter(),
                new GraphSourceJsonConverter(),
                new GraphTargetJsonConverter(),
            },
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
        try
        {
            GraphDataRaw raw = JsonSerializer.Deserialize<GraphDataRaw>(data, jsonSerializerOptions)!;
            raw.Edges = raw.Edges.Select(e => e with { Id = Guid.NewGuid() }).ToHashSet();
            // ?? throw new Exception("could not deserialize data");
            var microtopicDict = raw.Microtopics.GroupBy(m =>
            {
                if (m.Parent is null) throw new Exception($"microtopic({m.Id}) does not contain parent id");

                return m.Parent;
            }).ToDictionary(g => g.Key, g => new HashSet<Node>(g));

            var topicDict = raw.Topics.Select(t =>
                {
                    if (t.Parent is null) throw new Exception($"topic({t.Id}) does not contain parent id");
                    if (microtopicDict.Remove(t.Id, out var ms))
                    {
                        return new Topic
                        {
                            Value = t,
                            Microtopics = ms,
                        };
                    }

                    return new Topic()
                    {
                        Value = t,
                        Microtopics = new(),
                    };
                }).GroupBy(t => t.Value.Parent!)
                .ToDictionary(g => g.Key, g => new System.Collections.Generic.HashSet<Topic>(g));

            var unitSet = raw.Units.Select(u =>
            {
                if (topicDict.Remove(u.Id, out var ts))
                {
                    return new Unit()
                    {
                        Value = u,
                        Topics = ts,
                    };
                }

                return new Unit()
                {
                    Value = u,
                    Topics = new(),
                };
            }).ToHashSet();

            Graph uwo = new Graph()
            {
                UnitTree = unitSet,
                SpareTopics = topicDict.SelectMany(pair => pair.Value).ToHashSet(),
                SpareMicrotopics = microtopicDict.SelectMany(pair => pair.Value).ToHashSet(),
                Relations = raw.Edges,
            };

            _raw = raw;
            _unit = uwo;

            OnDataInitialized?.Invoke(this, EventArgs.Empty);
            
            return this;
        }
        catch
        {

            this._raw = new GraphDataRaw
            {
                Units = new HashSet<Node>(),
                Topics = new HashSet<Node>(),
                Microtopics = new HashSet<Node>(),
                Edges = new HashSet<Edge>()
            };
            this._unit = new Graph
            {
                UnitTree = new HashSet<Unit>(),
                SpareTopics = new HashSet<Topic>(),
                SpareMicrotopics = new HashSet<Node>(),
                Relations = new HashSet<Edge>()
            };
        }

        return this;
    }
    public string DumpJson()
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters  = {
                new GraphIdJsonConverter(),
                new GraphNameJsonConverter(),
                new GraphSourceJsonConverter(),
                new GraphTargetJsonConverter(),
            },
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
        
        return JsonSerializer.Serialize(_raw, jsonSerializerOptions);
    }

    public string GetCytoscapeData()
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters  = {
                new GraphIdJsonConverter(),
                new GraphNameJsonConverter(),
                new GraphSourceJsonConverter(),
                new GraphTargetJsonConverter(),
            }
        };
        List<CytoscapeInput> graphEls = new();
        graphEls.AddRange(Raw?.Microtopics.Select(m =>
        {
            if (!_mcColor.TryGetValue(m.Parent ?? "none", out string? color))
            {
                color = _mcColorSpare.FirstOrDefault() ?? "#000";
                _mcColorSpare.Remove(color);
                _mcColor[m.Parent?? "none"] = color;
            }

            return new CytoscapeInput()
            {
                Data = new CytoscapeNode()
                {
                    Id = m.Id,
                    Name = m.Name,
                    BackgroundColor = color,
                    TextColor = "rba(100,100,100)",
                    Parent = m.Parent?.Value,
                    ParentName = m.Parent != null
                        ? _raw.Topics.FirstOrDefault(t => t.Id == m.Parent)?.Name.Value
                        : null,
                },
                Position = m.Position,
            };
        }) ?? Enumerable.Empty<CytoscapeInput>());
        graphEls.AddRange(Raw?.Edges.Select(e =>
        {
            var data = new CytoscapeEdge()
            {
                Id = e.Id,
                Source = e.Source,
                Target = e.Target,
            };
            return new CytoscapeInput()
            {
                Data = data,
                Position = null,
            };
        }) ?? Enumerable.Empty<CytoscapeInput>());

        var result = JsonSerializer.Serialize(graphEls);
        return result;
    }
    
    public async Task<NewMicrotopic?> GetTopicId(GraphId? id = null)
    {
        var mv = _serviceProvider.GetService<AddMicrotopicsPopups>()
                 ?? throw new ArgumentNullException($"{nameof(AddMicrotopicsPopups)} class was not found in serice");

        if (id is not null)
        {
            mv.SetMicrotopicId(id);
        }

        await MopupService.Instance.PushAsync(mv);

        if (mv.PopupDismissedTask is null)
        {
            return null;
        }

        var rvalue =await mv.PopupDismissedTask;
        Console.WriteLine(rvalue);

        return rvalue;
    }

    private HashSet<string> _spareColors => new HashSet<string>()
    {
        "#44cb63",
        "#204f89",
        "#829868",
        "#3c5fd7",
        "#fda9aa",
        "#e623b1",
        "#f1ca20",
        "#c25ced",
        "#6b7f32",
        "#300e5d",
        "#f9c859",
        "#0e838f",
        "#c79505",
        "#dd93a5",
        "#01140b",
        "#e409ca",
        "#885c7a",
        "#752052",
        "#34571e",
        "#a28623",
        "#0fa97d",
        "#0b6dcd",
        "#0d073d",
        "#04b682",
        "#c32d33",
        "#6ee61d",
        "#d81fa9",
        "#0ede6f",
        "#718191",
        "#e032cd",
        "#fddb1a",
        "#7756d8",
        "#b0ffa5",
        "#763423",
        "#700411",
        "#eb5125",
        "#945e41",
        "#0b00b2",
        "#d51589",
        "#33333c",
    };
}
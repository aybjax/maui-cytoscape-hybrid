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

public partial class GraphService
{
    private readonly UndoService _undoService;
    Dictionary<string, string> _mcColor = new();
    private HashSet<string> _mcColorSpare = new();
    private readonly IServiceProvider _serviceProvider;
    private GraphDataRaw _raw = new()
    {
        Units = new (),
        Topics = new (),
        Microtopics = new (),
        Edges = new (),
        UnitEdges = new (),
    };
    public GraphDataRaw Raw => _raw;
    // public Graph Unit => _unit;
    public GraphService(IServiceProvider serviceProvider, UndoService undoService)
    {
        _serviceProvider = serviceProvider;
        _undoService = undoService;
        
        UndoService.MicrotopicCreationUndoEvent += OnMicrotopicCreationUndo;
        UndoService.MicrotopicDeletionUndoEvent += OnMicrotopicDeletionUndo;
        UndoService.EdgeCreationUndoEvent += OnEdgeCreationUndo;
        UndoService.EdgeDeletionUndoEvent += OnEdgeDeletionUndo;
        UndoService.MicrotopicUpdateUndoEvent += OnMicrotopicUpdatedUndo;
        UndoService.PositionUpdateUndoEvent += OnNodePositionUpdateUndo;
    }

    public GraphService InitializeData(string data)
    {
        _mcColorSpare = _spareColors.Select(e => e).ToHashSet();
        _mcColor = new();
        _undoService.Clear();
        
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
            raw.UnitEdges = raw.UnitEdges.Select(e => e with { Id = Guid.NewGuid() }).ToHashSet();
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
            
            _raw = raw;

            OnDataInitialized?.Invoke(this, EventArgs.Empty);
            
            return this;
        }
        catch(Exception e)
        {
            Application.Current?.MainPage?.DisplayAlert("File load error", e.Message, "OK");
            
            this._raw = new GraphDataRaw
            {
                Units = new (),
                Topics = new (),
                Microtopics = new (),
                Edges = new (),
                UnitEdges = new(),
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
        graphEls.AddRange(Raw?.Units.Where(u => u.IsVisible == true).Select(u =>
        {
            return new CytoscapeInput()
            {
                Data = new CytoscapeNode()
                {
                    Id = u.Id,
                    Name = u.Name,
                    TextColor = "rba(100,100,100)",
                    BackgroundColor = "black",
                },
                Position = u.Position,
            };
        }) ?? Enumerable.Empty<CytoscapeInput>());
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
                    Parent = m.ContainerId?.Value,
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
        graphEls.AddRange(Raw?.UnitEdges.Select(e =>
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
    public void Undo()
    {
        _undoService.Undo();
    }
    public void Redo()
    {
        _undoService.Redo();
    }
}
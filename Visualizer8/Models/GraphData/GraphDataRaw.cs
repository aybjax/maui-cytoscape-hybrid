using System.Text.Json.Serialization;

namespace Visualizer8.Models.GraphData;

public sealed class GraphDataRaw
{
    [JsonPropertyName("units")]
    public required HashSet<UnitNode> Units { get; set; }
    [JsonPropertyName("topics")]
    public required HashSet<Node> Topics { get; set; }
    [JsonPropertyName("microtopics")]
    public required HashSet<MicrotopicNode> Microtopics { get; set; }
    [JsonPropertyName("edges")]
    public required HashSet<Edge> Edges { get; set; }

    [JsonPropertyName("unit-edges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HashSet<Edge> UnitEdges { get; set; } = new();

}

public sealed record Topic
{
    [JsonPropertyName("value")]
    public required Node Value { get; set; }
    [JsonPropertyName("microtopics")]
    public required HashSet<Node> Microtopics { get; set; }
}

public sealed record Unit
{
    [JsonPropertyName("value")]
    public required Node Value { get; set; }
    [JsonPropertyName("topics")]
    public required HashSet<Topic> Topics { get; set; }
}
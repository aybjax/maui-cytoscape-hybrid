using System.Text.Json.Serialization;

namespace Visualizer8.Models.GraphData;

public sealed class GraphDataRaw
{
    [JsonPropertyName("units")]
    public required HashSet<Node> Units { get; set; }
    [JsonPropertyName("topics")]
    public required HashSet<Node> Topics { get; set; }
    [JsonPropertyName("microtopics")]
    public required HashSet<Node> Microtopics { get; set; }
    [JsonPropertyName("edges")]
    public required HashSet<Edge> Edges { get; set; }   
    
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
public sealed record Graph
{
    [JsonPropertyName("unit_tree")]
    public required HashSet<Unit> UnitTree { get; set; }
    [JsonPropertyName("spare_topics")]
    public required HashSet<Topic> SpareTopics { get; set; }
    [JsonPropertyName("spare_microtopics")]
    public required HashSet<Node> SpareMicrotopics { get; set; }
    [JsonPropertyName("relations")]
    public required HashSet<Edge> Relations { get; set; }
}
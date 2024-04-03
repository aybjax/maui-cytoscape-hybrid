using System.Text.Json.Serialization;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Models.GraphData;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Edge), "edge")]
[JsonDerivedType(typeof(Node), "node")]
public abstract record GraphElement
{
    public enum GraphType
    {
        Edge, Node
    }

    public abstract GraphType GetType();
}

public sealed record Edge(
    GraphId Id,
    GraphSource Source,
    GraphTarget Target
) : GraphElement
{
    public override GraphType GetType()
    {
        return GraphType.Edge;
    }

    [JsonPropertyName("id")]
    public GraphId Id { get; init; } = Id;
    [JsonPropertyName("source")]
    public GraphSource Source { get; init; } = Source;
    [JsonPropertyName("target")]
    public GraphTarget Target { get; init; } = Target;
}

public sealed record Node(
    GraphId Id,
    GraphName Name,
    GraphId? Parent = null,
    Position? Position = null
) : GraphElement
{
    public override GraphType GetType()
    {
        return GraphType.Node;
    }

    [JsonPropertyName("id")]
    public GraphId Id { get; init; } = Id;
    [JsonPropertyName("name")]
    public GraphName Name { get; init; } = Name;
    [JsonPropertyName("parent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GraphId? Parent { get; init; } = Parent;
}

public sealed record Position(
    float X,
    float Y
)
{
    [JsonPropertyName("x")]
    public float X { get; init; } = X;
    [JsonPropertyName("y")]
    public float Y { get; init; } = Y;
}

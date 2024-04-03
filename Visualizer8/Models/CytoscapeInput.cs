using System.Text.Json.Serialization;
using Visualizer8.Models.GraphData;

namespace Visualizer8.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CytoscapeEdge), "edge")]
[JsonDerivedType(typeof(CytoscapeNode), "node")]
public abstract record CytoscapeData
{}

public sealed record CytoscapeNode : CytoscapeData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("bg")]
    public required string BackgroundColor { get; init; }
    [JsonPropertyName("color")]
    public required string TextColor { get; init; }
    [JsonPropertyName("group")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Group { get; init; }
    [JsonPropertyName("parent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Parent { get; init; }
    [JsonPropertyName("parent_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentName { get; init; }
}

public sealed record CytoscapeEdge : CytoscapeData
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("source")]
    public required string Source { get; init; }

    [JsonPropertyName("target")]
    public required string Target { get; init; }
}

public sealed record CytoscapeInput
{
    [JsonPropertyName("data")]
    public required CytoscapeData Data { get; init; }
    [JsonPropertyName("position")]
    public required Position? Position { get; init; }
}
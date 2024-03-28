using Visualizer8.Models.GraphData;

namespace Visualizer8.Models;

public static class Mappers
{
    public static CytoscapeNode ToCytoscape(this Node node, BackgroundColor backgroundColor, TextColor textColor)
    {
        return new()
        {
            Id = node.Id,
            Name = node.Name,
            BackgroundColor = backgroundColor.ToString(),
            TextColor = textColor.ToString(),
        };
    }

    public static CytoscapeEdge ToCytoscape(this Edge edge)
    {
        return new()
        {
            Id = edge.Id,
            Source = edge.Source,
            Target = edge.Target,
        };
    }
}

public record Color(int R, int G, int B)
{
    public override string ToString()
    {
        return $"rgb({R},{G},{B})";
    }
}

public sealed record BackgroundColor(int R, int G, int B): Color(R, G, B);
public sealed record TextColor(int R, int G, int B): Color(R, G, B);
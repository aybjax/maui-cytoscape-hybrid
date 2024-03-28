using Visualizer8.Models.GraphData;

namespace Visualizer8.Extensions;

public sealed class EdgeEqualityComparer : IEqualityComparer<Edge>
{
    public bool Equals(Edge? x, Edge? y) => x?.Id == y?.Id;
    public int GetHashCode(Edge obj) => obj.Id.GetHashCode();
}

public sealed class NodeEqualityComparer: IEqualityComparer<Node>
{
    public bool Equals(Node? x, Node? y) => x?.Id == y?.Id;

    public int GetHashCode(Node obj) => obj.GetHashCode();
}

public sealed class TopicEqualityComparer: IEqualityComparer<Topic>
{
    public bool Equals(Topic? x, Topic? y) => x?.Value.Id == y?.Value.Id;

    public int GetHashCode(Topic obj) => obj.Value.Id.GetHashCode();
}

public sealed class UnitEqualityComparer: IEqualityComparer<Unit>
{
    public bool Equals(Unit? x, Unit? y) => x?.Value.Id == y?.Value.Id;

    public int GetHashCode(Unit obj) => obj.Value.Id.GetHashCode();
}
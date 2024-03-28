namespace Visualizer8.Models.GraphDataPrimitives;
using ValueOf;

public sealed class GraphName: ValueOf<string, GraphName>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new GraphNameCannotBeNullException();
        }
    }

    public static implicit operator GraphName(string name)
    {
        return From(name);
    }

    public static implicit operator string(GraphName name)
    {
        return name.Value;
    }

    public sealed class GraphNameCannotBeNullException : Exception
    {
        public GraphNameCannotBeNullException(): base("Graph name cannot be empty")
        { }
    }
}

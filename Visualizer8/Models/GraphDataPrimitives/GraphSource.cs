namespace Visualizer8.Models.GraphDataPrimitives;
using ValueOf;

public sealed class GraphSource : ValueOf<string, GraphSource>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new GraphSourceCannotBeEmptyException();
        }
    }

    public static implicit operator GraphSource(string source)
    {
        return From(source);
    }

    public static implicit operator string(GraphSource source)
    {
        return source.Value;
    }

    public sealed class GraphSourceCannotBeEmptyException : Exception
    {
        public GraphSourceCannotBeEmptyException(): base("Graph source cannot be empty")
        { }
    }
}
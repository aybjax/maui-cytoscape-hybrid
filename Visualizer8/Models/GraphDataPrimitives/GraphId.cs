namespace Visualizer8.Models.GraphDataPrimitives;
using ValueOf;

public sealed class GraphId: ValueOf<string,GraphId>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new GraphIdCannotBeNullException();
        }
    }

    public static implicit operator GraphId(Guid guid)
    {
        return From(guid.ToString());
    }

    public static implicit operator GraphId(string str)
    {
        return From(str);
    }

    public static implicit operator string(GraphId id)
    {
        return id.Value;
    }

    public sealed class GraphIdCannotBeNullException : Exception
    {
        public GraphIdCannotBeNullException(): base("Graph Id object cannot be empty")
        {}
    }
}
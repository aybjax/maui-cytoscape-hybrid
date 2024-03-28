namespace Visualizer8.Models.GraphDataPrimitives;
using ValueOf;

public sealed class GraphTarget: ValueOf<string, GraphTarget>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new GraphTargetCannotBeEmptyException();
        }
    }

    public static implicit operator GraphTarget(string target)
    {
        return From(target);
    }

    public static implicit operator string(GraphTarget target)
    {
        return target.Value;
    }

    public sealed class GraphTargetCannotBeEmptyException : Exception
    {
        public GraphTargetCannotBeEmptyException(): base("Graph target cannot be empty")
        {
            
        }
    }
}
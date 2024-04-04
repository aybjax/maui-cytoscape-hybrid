using Visualizer8.Models.GraphData;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Models.Input;

public record NewMicrotopic(GraphId Id, GraphName Name, GraphId ParentId, Position Position);
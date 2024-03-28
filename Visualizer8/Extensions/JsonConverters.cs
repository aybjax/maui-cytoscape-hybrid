using System.Text.Json;
using System.Text.Json.Serialization;
using Visualizer8.Models.GraphDataPrimitives;

namespace Visualizer8.Extensions;


public class GraphIdJsonConverter: JsonConverter<GraphId>
{
    public override GraphId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return GraphId.From(reader.GetString() ?? "");
    }

    public override void Write(Utf8JsonWriter writer, GraphId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}

public class GraphNameJsonConverter : JsonConverter<GraphName>
{
    public override GraphName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return GraphName.From(reader.GetString() ?? "");
    }

    public override void Write(Utf8JsonWriter writer, GraphName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}

public class GraphSourceJsonConverter : JsonConverter<GraphSource>
{
    public override GraphSource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return GraphSource.From(reader.GetString() ?? "");
    }

    public override void Write(Utf8JsonWriter writer, GraphSource value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}

public class GraphTargetJsonConverter : JsonConverter<GraphTarget>
{
    public override GraphTarget? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return GraphTarget.From(reader.GetString() ?? "");
    }

    public override void Write(Utf8JsonWriter writer, GraphTarget value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
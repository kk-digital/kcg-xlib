using MongoDB.Bson.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lib;

// this file contains classes for deserializing/serializing to json, and for reading from/to mongo as well.

// class to serialize to MongoDB
public class Uuid64BsonSerializer : IBsonSerializer<Uid64>
{
    public Type ValueType => typeof(Uid64);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Uid64 value)
    {
        // Serialize the Uid64 as a ulong
        context.Writer.WriteInt64((long)value.ToUInt64());
    }

    public Uid64 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        // Deserialize the ulong into a Uid64 object
        long intValue = context.Reader.ReadInt64();
        return Uid64.FromUInt64((ulong)intValue);
    }

    // Explicit interface implementation for non-generic interface
    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        Serialize(context, args, (Uid64)value);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }
}

public class UidJsonConverter : JsonConverter<Uid64>
{
    public override Uid64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string uuidString = reader.GetString();
        if (uuidString == null)
        {
            throw new JsonException("Uid string is null");
        }
        return Uid64.FromFormattedString(uuidString);
    }

    public override void Write(Utf8JsonWriter writer, Uid64 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToFormattedStr());
    }
}

public class UuidListJsonConverter : JsonConverter<List<Uid64>>
{
    public override List<Uid64> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<Uid64> uuids = new List<Uid64>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            string uuidString = reader.GetString();
            if (uuidString == null)
            {
                throw new JsonException("Uid string is null");
            }

            uuids.Add(Uid64.FromFormattedString(uuidString));
        }

        return uuids;
    }

    public override void Write(Utf8JsonWriter writer, List<Uid64> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (Uid64 Uid in value)
        {
            writer.WriteStringValue(Uid.ToFormattedStr());
        }

        writer.WriteEndArray();
    }
}
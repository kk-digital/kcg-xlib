using MongoDB.Bson.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lib;

// this file contains classes for deserializing/serializing to json, and for reading from/to mongo as well.

// class to serialize to MongoDB
public class Uuid64BsonSerializer : IBsonSerializer<Uuid64>
{
    public Type ValueType => typeof(Uuid64);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Uuid64 value)
    {
        // Serialize the Uuid64 as a ulong
        context.Writer.WriteInt64((long)value.ToUInt64());
    }

    public Uuid64 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        // Deserialize the ulong into a Uuid64 object
        long intValue = context.Reader.ReadInt64();
        return Uuid64.FromUInt64((ulong)intValue);
    }

    // Explicit interface implementation for non-generic interface
    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        Serialize(context, args, (Uuid64)value);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }
}

public class UuidJsonConverter : JsonConverter<Uuid64>
{
    public override Uuid64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string uuidString = reader.GetString();
        if (uuidString == null)
        {
            throw new JsonException("UUID string is null");
        }
        return Uuid64.FromFormattedString(uuidString);
    }

    public override void Write(Utf8JsonWriter writer, Uuid64 value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToFormattedStr());
    }
}

public class UuidListJsonConverter : JsonConverter<List<Uuid64>>
{
    public override List<Uuid64> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<Uuid64> uuids = new List<Uuid64>();

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
                throw new JsonException("UUID string is null");
            }

            uuids.Add(Uuid64.FromFormattedString(uuidString));
        }

        return uuids;
    }

    public override void Write(Utf8JsonWriter writer, List<Uuid64> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (Uuid64 uuid in value)
        {
            writer.WriteStringValue(uuid.ToFormattedStr());
        }

        writer.WriteEndArray();
    }
}
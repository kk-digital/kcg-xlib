using System.Text.Json;
using System.Text.Json.Serialization;
using UtilityIO;

namespace FileLib
{
    public class FilePathJsonConverter : JsonConverter<FilePath>
    {
        public override void Write(Utf8JsonWriter writer, FilePath value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.GetPath());
        }

        public override FilePath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            string path = reader.GetString();
            if (!PathUtils.IsAbsolutePath(path))
            {
                path = PathUtils.GetFullPath(path);
            }
            return new FilePath(path);
        }
    }
}
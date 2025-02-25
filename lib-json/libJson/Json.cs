using System.Text.Json;

namespace libJson
{
    /// <summary>
    /// Provides JSON serialization, deserialization, and validation functionalities.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Serializes an object into a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string Serialize<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Serialization failed.", ex);
            }
        }

        /// <summary>
        /// Serializes an object into a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">The JsonSerializerOptions to use.</param>
        /// <returns>A JSON string representation of the object.</returns>
        public static string Serialize<T>(T obj, JsonSerializerOptions options)
        {
            try
            {
                return JsonSerializer.Serialize(obj, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Serialization failed.", ex);
            }
        }

        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An object of type T.</returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Deserialization failed.", ex);
            }
        }
        
        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize into.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="options">The JsonSerializerOptions to use.</param>
        /// <returns>An object of type T.</returns>
        public static T Deserialize<T>(string json, JsonSerializerOptions options)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Deserialization failed.", ex);
            }
        }

        /// <summary>
        /// Validates whether a given string is valid JSON.
        /// </summary>
        /// <param name="jsonString">The string to validate.</param>
        /// <returns>True if the string is valid JSON; otherwise, false.</returns>
        public static bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return false;
            }

            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
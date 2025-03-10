using System.Text.Json.Serialization;

namespace UtilityHttpServer;
class ServerStatus {
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

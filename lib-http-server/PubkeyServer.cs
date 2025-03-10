using System.Text.Json.Serialization;

namespace UtilityHttpServer;

class PubkeyServer {
    [JsonPropertyName("type")]
    public string Type;
    [JsonPropertyName("pubkey")]
    public string Pubkey;
}

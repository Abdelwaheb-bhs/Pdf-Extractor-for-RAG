using System.Text.Json.Serialization;

namespace RAGProject.Models
{
    public class RagSearchResponse
    {
        [JsonPropertyName("result")]
        public List<SearchResult> Results { get; set; } = new();
    }

    public class SearchResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } // Change from int to string

        [JsonPropertyName("score")]
        public float Score { get; set; } // Ensure Score is included

        [JsonPropertyName("payload")]
        public Dictionary<string, object> Payload { get; set; } = new();
    }
}


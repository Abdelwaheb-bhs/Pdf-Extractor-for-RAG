using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nexia.Qdrant;

/// Base class for Qdrant response schema.
internal abstract class QdrantResponse
{
   
    /// Response status
  
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; set; }

  
    /// Response time
  
    [JsonPropertyName("time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Time { get; set; }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nexia.DataFormats;

public class FileContent
{
    [JsonPropertyOrder(0)]
    [JsonPropertyName("sections")]
    public List<Chunk> Sections { get; set; } = [];

    [JsonPropertyOrder(1)]
    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; }

    public FileContent(string mimeType)
    {
        this.MimeType = mimeType;
    }
}
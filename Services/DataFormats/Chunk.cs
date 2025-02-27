using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nexia.DataFormats;
public class Chunk
{
    // Metadata keys
    private const string MetaSentencesAreComplete = "completeSentences";
    private const string MetaPageNumber = "pageNumber";

    
    /// Text page number/Audio segment number/Video scene number
    
    [JsonPropertyOrder(0)]
    [JsonPropertyName("number")]
    public int Number { get; }

    
    /// Page text content
    
    [JsonPropertyOrder(1)]
    [JsonPropertyName("content")]
    public string Content { get; set; }

    
    /// Optional metadata attached to the section.
    /// Values are JSON strings to be serialized/deserialized.
    /// Examples:
    /// - sentences are complete y/n
    /// - page number
    
    [JsonPropertyOrder(10)]
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; }

    [JsonIgnore]
    public bool IsSeparator { get; set; }

    
    /// true: jomla loula/lekhra mosta9alin 3ala b3adhhom, loula mahiyech kmelit li 9balha,
    //  w le5ra youfa beha ma3na
    ///       win toufa section (exp: PowerPoint, Excel).
    /// false: ken jomla loula tabda mili 9balha or jomla lekhra tekmel m3a lemba3dha.
   
    [JsonIgnore]
    public bool SentencesAreComplete
    {
        get
        {
            return this.Metadata.TryGetValue(MetaPageNumber, out var value) && JsonSerializer.Deserialize<bool>(value);
        }
    }

    [JsonIgnore]
    public int PageNumber
    {
        get
        {
            if (this.Metadata.TryGetValue(MetaPageNumber, out var value))
            {
                return JsonSerializer.Deserialize<int>(value);
            }

            return -1;
        }
    }


    /// Create new instance
    ///"number" Position within the parent content container
    /// "text"nText content
    public Chunk(string? text, int number)
    {
        this.Content = text ?? string.Empty;
        this.Number = number;
        this.Metadata = new();
    }

    public Chunk(StringBuilder text, int number)
    {
        this.Content = text.ToString();
        this.Number = number;
        this.Metadata = new();
    }
     public Chunk(string? text, int number, Dictionary<string, string> metadata)
    {
        this.Content = text ?? string.Empty;
        this.Number = number;
        this.Metadata = metadata;
    }
    
    /// Metadata builder
    /// "sentencesAreComplete" ken jomla loula/lekhra sentence tkoun kmalet li9balha/liba3dha fi next section
    /// "pageNumber" Number of the page where the content is extracted from
    public static Dictionary<string, string> Meta(
        bool? sentencesAreComplete = null,
        int? pageNumber = null)
    {
        var result = new Dictionary<string, string>();

        if (sentencesAreComplete.HasValue)
        {
            result.Add(MetaSentencesAreComplete, JsonSerializer.Serialize(sentencesAreComplete.Value));
        }

        if (pageNumber.HasValue)
        {
            result.Add(MetaPageNumber, JsonSerializer.Serialize(pageNumber.Value));
        }

        return result;
    }
}
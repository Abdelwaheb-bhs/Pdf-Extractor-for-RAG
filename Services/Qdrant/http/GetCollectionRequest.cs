using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Nexia.Qdrant;

internal sealed class GetCollectionsRequest
{
    /// <summary>
    /// Name of the collection to request vectors from
    /// </summary>
    [JsonIgnore]
    public string Collection { get; set; }

    public static GetCollectionsRequest Create(string collectionName)
    {
        return new GetCollectionsRequest(collectionName);
    }

    public HttpRequestMessage Build()
    {
        return http.HttpRequest.CreateGetRequest($"collections/{this.Collection}");
    }

    #region private ================================================================================

    private GetCollectionsRequest(string collectionName)
    {
        this.Collection = collectionName;
    }

    #endregion
}
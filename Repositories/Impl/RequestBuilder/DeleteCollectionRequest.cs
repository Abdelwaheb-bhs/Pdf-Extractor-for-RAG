using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Nexia.Diagnostic;
namespace Nexia.Qdrant;

internal sealed class DeleteCollectionRequest
{
    public static DeleteCollectionRequest Create(string collectionName)
    {
        return new DeleteCollectionRequest(collectionName);
    }

    public void Validate()
    {
    }

    public HttpRequestMessage Build()
    {
        Verify.NotNullOrWhiteSpace(this._collectionName, "collectionName");
        return http.HttpRequest.CreateDeleteRequest($"collections/{this._collectionName}?timeout=30");
    }

    #region private ================================================================================

    private readonly string _collectionName;

    private DeleteCollectionRequest(string collectionName)
    {
        this._collectionName = collectionName;
    }

    #endregion
}
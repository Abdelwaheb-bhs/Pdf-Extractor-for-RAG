using RAGProject.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SharpToken;
using RAGProject.Services.Interfaces;
using Nexia.Qdrant;
using Nexia.Embeddings;
using Nexia.Memory;
namespace RAGProject.Services.Impl
{
    public class EmbeddingService : IEmbeddingService
    {
     private readonly ITextEmbeddingGenerationService _embeddingGenerator;
    private readonly IQdrantService _storage;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticTextMemory"/> class.
    /// </summary>
    /// <param name="storage">The memory store to use for storing and retrieving data.</param>
    /// <param name="embeddingGenerator">The text embedding generator to use for generating embeddings.</param>
    public EmbeddingService(
        IQdrantService storage,
        ITextEmbeddingGenerationService embeddingGenerator)
    {
        this._embeddingGenerator = embeddingGenerator;
        this._storage = storage;
    }

    /// <inheritdoc/>
    public async Task<string> SaveInformationAsync(
        string collection,
        string text,
        string id,
        string? description = null,
        string? additionalMetadata = null,
       
        CancellationToken cancellationToken = default)
    {
        
      
        var embeddings = await this._embeddingGenerator.GenerateEmbeddingsAsync(new List<string> { text }, cancellationToken).ConfigureAwait(false);

        // Ensure there is at least one embedding
        if (embeddings == null )
        {
            throw new InvalidOperationException("Embedding generation failed: No embeddings were returned.");
        }

        // Get the first embedding
        var embedding = embeddings[0];

        MemoryRecord data = MemoryRecord.LocalRecord(
            id: id,
            text: text,
            description: description,
            additionalMetadata: additionalMetadata,
            embedding: embedding);

        if (!await this._storage.DoesCollectionExistAsync(collection, cancellationToken).ConfigureAwait(false))
        {
            await this._storage.CreateCollectionAsync(collection, cancellationToken).ConfigureAwait(false);
        }

        return await this._storage.UpsertAsync(collection, data, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<string> SaveReferenceAsync(
        string collection,
        string text,
        string externalId,
        string externalSourceName,
        string? description = null,
        string? additionalMetadata = null,
     
        CancellationToken cancellationToken = default)
    {
        var embeddings = await this._embeddingGenerator.GenerateEmbeddingsAsync(new List<string> { text }, cancellationToken).ConfigureAwait(false);

            // Ensure there is at least one embedding
            if (embeddings == null || embeddings.Count == 0)
            {
                throw new InvalidOperationException("Embedding generation failed: No embeddings were returned.");
            }

            // Get the first embedding
            var embedding = embeddings[0];
        var data = MemoryRecord.ReferenceRecord(externalId: externalId, sourceName: externalSourceName, description: description,
            additionalMetadata: additionalMetadata, embedding: embedding);

        if (!await this._storage.DoesCollectionExistAsync(collection, cancellationToken).ConfigureAwait(false))
        {
            await this._storage.CreateCollectionAsync(collection, cancellationToken).ConfigureAwait(false);
        }

        return await this._storage.UpsertAsync(collection, data, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MemoryQueryResult?> GetAsync(
        string collection,
        string key,
        bool withEmbedding = false,
      
        CancellationToken cancellationToken = default)
    {
        MemoryRecord? record = await this._storage.GetAsync(collection, key, withEmbedding, cancellationToken).ConfigureAwait(false);

        if (record is null) { return null; }

        return MemoryQueryResult.FromMemoryRecord(record, 1);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(
        string collection,
        string key,
       
        CancellationToken cancellationToken = default)
    {
        await this._storage.RemoveAsync(collection, key, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<MemoryQueryResult> SearchAsync(
        string collection,
        string query,
        int limit = 1,
        double minRelevanceScore = 0.0,
        bool withEmbeddings = false,
       CancellationToken cancellationToken = default)
    {
        var queryEmbeddings = await this._embeddingGenerator.GenerateEmbeddingsAsync(
    new List<string> { query }, 
    cancellationToken).ConfigureAwait(false);

        // Ensure there is at least one embedding
        if (queryEmbeddings == null || queryEmbeddings.Count == 0)
        {
            throw new InvalidOperationException("Embedding generation failed: No embeddings were returned.");
        }

        // Extract the first embedding
        ReadOnlyMemory<float> queryEmbedding = queryEmbeddings[0]; 

        if ((await this._storage.DoesCollectionExistAsync(collection, cancellationToken).ConfigureAwait(false)))
        {
            IAsyncEnumerable<(MemoryRecord, double)> results = this._storage.GetNearestMatchesAsync(
                collectionName: collection,
                embedding: queryEmbedding,
                limit: limit,
                minRelevanceScore: minRelevanceScore,
                withEmbeddings: withEmbeddings,
                cancellationToken: cancellationToken);

            await foreach ((MemoryRecord, double) result in results.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return MemoryQueryResult.FromMemoryRecord(result.Item1, result.Item2);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<IList<string>> GetCollectionsAsync( CancellationToken cancellationToken = default)
    {
        return await this._storage.GetCollectionsAsync(cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}
}

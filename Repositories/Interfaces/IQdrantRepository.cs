using System.Collections.Generic;
using System.Threading.Tasks;
using RAGProject.Models;
namespace Nexia.Qdrant

{
    public interface IQdrantRepository
       {
        /// "cancellationToken" to monitor for cancellation requests. The default is "CancellationToken.None"
        public IAsyncEnumerable<QdrantVectorRecord> GetVectorsByIdAsync(string collectionName, IEnumerable<string> pointIds, bool withVectors = false,
        CancellationToken cancellationToken = default);
        public Task<QdrantVectorRecord?> GetVectorByPayloadIdAsync(string collectionName, string metadataId, bool withVector = false, CancellationToken cancellationToken = default);

        public Task DeleteVectorByPayloadIdAsync(string collectionName, string metadataId, CancellationToken cancellationToken = default);
        public Task DeleteVectorsByIdAsync(string collectionName, IEnumerable<string> pointIds, CancellationToken cancellationToken = default);
        public Task UpsertVectorsAsync(string collectionName, IEnumerable<QdrantVectorRecord> vectorData, CancellationToken cancellationToken = default);

        public IAsyncEnumerable<(QdrantVectorRecord, double)> FindNearestInCollectionAsync(
        string collectionName,
        ReadOnlyMemory<float> target,
        double threshold,
        int top = 1,
        bool withVectors = false,
        IEnumerable<string>? requiredTags = null,
        CancellationToken cancellationToken = default);
        public Task CreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

        public Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);

        public Task<bool> DoesCollectionExistAsync(string collectionName, CancellationToken cancellationToken = default);

        public IAsyncEnumerable<string> ListCollectionsAsync(CancellationToken cancellationToken = default);
    }
}
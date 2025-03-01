using System.Diagnostics.CodeAnalysis;

namespace Nexia.Embeddings;

/// <summary>
/// Represents a generator of text embeddings of type <c>float</c>.
/// </summary>
public interface ITextEmbeddingGenerationService : IEmbeddingGenerationService<string, float>;
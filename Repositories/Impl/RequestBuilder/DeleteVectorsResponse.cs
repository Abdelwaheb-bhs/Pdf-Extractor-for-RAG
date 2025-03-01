using System.Diagnostics.CodeAnalysis;

namespace Nexia.Qdrant;

/// Empty qdrant response for requests that return nothing but status / error.
internal sealed class DeleteVectorsResponse : QdrantResponse;

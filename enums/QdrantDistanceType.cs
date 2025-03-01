using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Nexia.Qdrant;

/// The vector distance type used by Qdrant.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QdrantDistanceType
{
   
    /// Cosine of the angle between vectors, aka dot product scaled by magnitude. Cares only about angle difference.
    Cosine,

    /// Consider angle and distance (magnitude) of vectors.
    DotProduct,

    /// Pythagorean(theorem) distance of 2 multidimensional points.
    Euclidean,

    /// Sum of absolute differences between points across all the dimensions.
    Manhattan,
    /// Assuming only the most significant dimension is relevant.
    Chebyshev,
    /// Generalization of Euclidean and Manhattan.
    Minkowski,
}
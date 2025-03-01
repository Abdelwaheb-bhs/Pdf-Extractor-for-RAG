
namespace RAGProject.Models
{
    public class Vector
    {
        public Guid Id { get; set; }
        public List<float> Embeddings { get; set; }
        public Metadata Metadata { get; set; }
        public Vector(Guid id, List<float> embeddings , Metadata metadata)
        {
            Id = id;
            Embeddings = embeddings;
            Metadata = metadata;
        }
    }
}
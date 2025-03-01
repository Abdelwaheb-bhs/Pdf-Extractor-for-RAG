namespace RAGProject.Models
{
    public class GetCollectionsResponse
    {
        public string Status { get; set; }  // To match the "status" field in the response
        public QdrantResult Result { get; set; }
        public double Time { get; set; }  // If you want to capture the "time" field
    }

    public class QdrantResult
    {
        public List<Collection> Collections { get; set; }
    }

    public class Collection
    {
        public string Name { get; set; }
    }

}
namespace RAGProject.Models
{
    public class QdrantScrollResponseModel
    {
        // Ensure this matches the JSON structure you are receiving
        public Result result { get; set; }
    }

    public class Result
    {
        // Change to a List to accommodate an array of points
        public List<QdrantPoint> points { get; set; }
    }

    public class QdrantPoint
    {
        // Define properties of each point
        public string Id { get; set; }
        // Add any other properties of the point if necessary
    }
}
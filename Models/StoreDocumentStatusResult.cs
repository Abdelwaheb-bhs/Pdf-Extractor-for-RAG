
namespace RAGProject.Models
{
    public class StoreDocumentStatusResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        private StoreDocumentStatusResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static StoreDocumentStatusResult CreateSuccess(string message) => new StoreDocumentStatusResult(true, message);
        public static StoreDocumentStatusResult CreateFailure(string message) => new StoreDocumentStatusResult(false, message);
    }
}
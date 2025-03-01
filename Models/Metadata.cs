
namespace RAGProject.Models
{
    public class Metadata
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Text { get; set; } // Store the actual document text
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Metadata(string title, string category , string text)
        {
            Title = title;
            Category = category;
            Text = text;
        }
    }
}

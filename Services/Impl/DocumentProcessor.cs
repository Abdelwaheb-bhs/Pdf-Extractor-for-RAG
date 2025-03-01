using System.Text.RegularExpressions;
using RAGProject.Services.Interfaces;
using SharpToken;

namespace RAGProject.Services.Impl
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly GptEncoding _tokenizer;

        public DocumentProcessor()
        {
            _tokenizer = GptEncoding.GetEncoding("cl100k_base"); // Use the appropriate encoding
        }

        public string ExtractText(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            string text = string.Empty;
            switch (ext)
            {
                case ".txt":
                    text = File.ReadAllText(filePath);
                    break;
                case ".pdf":
                    text = "PDF extraction not implemented.";
                    break;
                case ".docx":
                    text = "DOCX extraction not implemented.";
                    break;
                default:
                    text = "Unsupported file format.";
                    break;
            }
            return text;
        }

        public List<string> SplitTextIntoChunks(string text, int maxTokenLength = 512)
        {
            string cleanedText = Regex.Replace(text, @"\s+", " ").Trim();
            var chunks = new List<string>();

            // Tokenize the cleaned text to count the tokens
            var encodedText = _tokenizer.Encode(cleanedText);
            int totalTokens = encodedText.Count;

            // Split into chunks based on the number of tokens
            for (int i = 0; i < totalTokens; i += maxTokenLength)
            {
                int chunkTokenLength = Math.Min(maxTokenLength, totalTokens - i);
                var subTokens = encodedText.GetRange(i, chunkTokenLength);
                var chunkText = _tokenizer.Decode(subTokens);  // Decode the tokens back to text
                chunks.Add(chunkText);
            }

            return chunks;
        }
    }
}

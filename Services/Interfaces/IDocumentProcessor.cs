using System.Collections.Generic;

namespace RAGProject.Services.Interfaces
{
    public interface IDocumentProcessor
    {
        string ExtractText(string filePath);
        List<string> SplitTextIntoChunks(string text, int chunkSize = 500);
    }
}

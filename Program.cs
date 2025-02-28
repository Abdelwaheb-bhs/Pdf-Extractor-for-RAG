
using UglyToad.PdfPig;
using Qdrant.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nexia.Memory;
using Nexia.Embeddings;
using Nexia.Qdrant;

class Program
{
    static async Task Main(string[] args)
    {
        string qdrantEndpoint = "https://20b30264-b016-4cce-8b25-01596e843473.us-east4-0.gcp.cloud.qdrant.io:6333";
        string collectionName = "little_prince";
        string qdrantApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2Nlc3MiOiJtIn0.lMSyLO6ctdMEZ0HpGJVn5t9_lhJQCxYbson8xN1GYb0";
        
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("api-key", qdrantApiKey);

      
        
        var embeddingService = OnnxEmbeddingGeneration.Create("embModel/model.onnx", "embModel/vocab.txt");
        var memoryStore = new QdrantMemoryStore(httpClient, vectorSize: 384, qdrantEndpoint);
        var memory = new NexiaMemory(memoryStore, embeddingService);

        string pdfPath = "Little-Prince-final-text.pdf";
        if (!await IsDocumentProcessed(memory, collectionName, pdfPath))
        {
            Console.WriteLine("Processing PDF for the first time...");
            await IngestPdf(memory, collectionName, pdfPath);
        }
        else
        {
            Console.WriteLine("PDF already processed, skipping ingestion.");
        }

        Console.WriteLine("Ask a question:");
        string? query = Console.ReadLine();
        if (query != null)
        {
            await QueryRAG(memory, collectionName, query);
        }
        else
        {
            Console.WriteLine("No question provided!");
        }

        Console.WriteLine("Done!");
    }

    static async Task<bool> IsDocumentProcessed(NexiaMemory memory, string collectionName, string pdfPath)
    {
        // Use a unique identifier for the document (e.g., filename or a hash of the content)
        string docId = $"metadata_{Path.GetFileName(pdfPath)}";
        var searchResult = await memory.GetAsync(collectionName, docId);
        return searchResult != null; // If metadata exists, document is already processed
    }

    static async Task IngestPdf(NexiaMemory memory, string collectionName, string pdfPath)
    {
        using var pdf = PdfDocument.Open(pdfPath);
        string text = string.Join(" ", pdf.GetPages().Select(p => p.Text));

        var chunks = text.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim() + ".").Where(c => c.Length > 20);

        int id = 0;
        foreach (var chunk in chunks)
        {
            await memory.SaveInformationAsync(
                collection: collectionName,
                text: chunk,
                id: $"chunk_{id++}",
                description: "PDF content chunk"
            );
            Console.WriteLine($"Ingested: {chunk.Substring(0, Math.Min(50, chunk.Length))}...");
        }
    }

    static async Task QueryRAG(NexiaMemory memory, string collectionName, string query)
{
    var results = memory.SearchAsync(collectionName, query, limit: 3, minRelevanceScore: 0.5);
    string context = "";
    int resultCount = 0;
    await foreach (var result in results)
    {
        resultCount++;
        context += result.Metadata.Text + " ";
        Console.WriteLine($"Retrieved [{result.Relevance}]: {result.Metadata.Text.Substring(0, Math.Min(50, result.Metadata.Text.Length))}...");
    }
    Console.WriteLine($"Found {resultCount} results. Context: {context}");
    if (string.IsNullOrEmpty(context))
    {
        Console.WriteLine("I don't know! No relevant context found.");
        return;
    }

    

   
}
}


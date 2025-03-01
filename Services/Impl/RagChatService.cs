using RAGProject.Services.Interfaces;
using Nexia.Qdrant;
using Nexia.Memory;
namespace RAGProject.Services.Impl
{
    public class RagChatService : IRagChatService
    {
        private readonly IQdrantRepository _qdrantRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IMistralService _mistralService;

        public RagChatService(IQdrantRepository qdrantRepository, IEmbeddingService embeddingService, IMistralService mistralService)
        {
            _qdrantRepository = qdrantRepository;
            _embeddingService = embeddingService;
            _mistralService = mistralService;
        }

        public async Task<string> GenerateWithRag(string userInput, string collectionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userInput))
                    throw new ArgumentException("User input cannot be empty.");

                if (string.IsNullOrWhiteSpace(collectionName))
                    throw new ArgumentException("Collection name cannot be empty.");

                // Generate Embeddings
                var promptEmbedding = new List<MemoryQueryResult>();
                await foreach (var result in _embeddingService.SearchAsync(userInput, collectionName, 10, 0.5, false, CancellationToken.None))
                {
                    promptEmbedding.Add(result);
                }
                if (promptEmbedding.Count == 0)
                    throw new Exception("Failed to generate embeddings.");
                Console.WriteLine("Embeddings generated: " + string.Join(", ", promptEmbedding));

                // Retrieve Relevant Texts
                var embeddings = promptEmbedding
                    .Where(e => e.Embedding.HasValue)
                    .SelectMany(e => e.Embedding.Value.ToArray())
                    .ToArray();
                var relevantTexts = new List<QdrantVectorRecord>();
                await foreach (var (record, _) in _qdrantRepository.FindNearestInCollectionAsync(collectionName, new ReadOnlyMemory<float>(embeddings), 0.5, 10, false, null, CancellationToken.None))
                {
                    relevantTexts.Add(record);
                }
                if (relevantTexts == null )
                    throw new Exception("No relevant texts found in the knowledge base.");
                Console.WriteLine("Relevant texts retrieved." + relevantTexts);

                // Format Prompt
                string relevantInformation = string.Join("\n\n", relevantTexts);
                var prompt = $"Context information is below:\n---------------------\n{relevantInformation}\n---------------------\ndoes the context above has informations about the query,\nQuery: {userInput}\nAnswer:";
                Console.WriteLine("Prompt generated." + prompt);
                // Get Response from Mistral
                var response = await _mistralService.GetMistralResponse(prompt);
                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("Failed to generate a response from Mistral.");

                return response;
            }
            catch (ArgumentException ex)
            {
                return $"Input Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Processing Error: {ex.Message}";
            }
        }
    }
}
using System.Threading.Tasks;

public interface IRagChatService
{
    Task<string> GenerateWithRag(string userInput, string collectionName);
}

using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetEnv;
namespace RAGProject.Services.Impl
{
    public class MistralService : IMistralService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly List<dynamic> _conversationHistory;

        public MistralService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Env.Load("./.env"); // Load from project root
            _apiKey = Env.GetString("MISTRAL_API_KEY"); // Fetch API key from .env

            _conversationHistory = new List<dynamic>();

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MistralApiClient/1.0");
        }

        public async Task<string> GetMistralResponse(string userInput)
        {
            // Add user input to the conversation history
            _conversationHistory.Add(new { role = "user", content = userInput });

            var payload = new
            {
                model = "open-mistral-7b",
                messages = _conversationHistory,
                temperature = 0.7,
                top_p = 1,
                max_tokens = 512,
                stream = false,
                safe_prompt = false,
                random_seed = 1337
            };

            try
            {
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.mistral.ai/v1/chat/completions", httpContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponseObj = JObject.Parse(responseContent);

                string messageContent = jsonResponseObj["choices"][0]["message"]["content"].ToString();

                // Add Mistral's response to the conversation history
                _conversationHistory.Add(new { role = "assistant", content = messageContent });

                return messageContent;
            }
            catch (HttpRequestException e)
            {
                return $"Request error: {e.Message}";
            }
        }
    }
}
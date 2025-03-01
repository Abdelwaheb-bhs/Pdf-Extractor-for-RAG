using Microsoft.AspNetCore.Mvc;
using RAGProject.DTO;

namespace RAGProject.Controllers
{
    [ApiController]
    [Route("api/mistral")]
    public class MistralController : ControllerBase
    {
        private readonly IMistralService _mistralService;
        private readonly IRagChatService _ragChatService;

        public MistralController(IMistralService mistralService, IRagChatService ragChatService)
        {
            _mistralService = mistralService;
            _ragChatService = ragChatService;
        }

        [HttpPost("chatWithLLM")]
        public async Task<IActionResult> Chat([FromBody] MistralRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Message cannot be empty.");
            }

            string response = await _mistralService.GetMistralResponse(request.Message);
            return Ok(new { response });
        }
    
        [HttpPost("generateWithRag")]
        public async Task<IActionResult> GenerateWithRag([FromBody] RagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserInput) || string.IsNullOrWhiteSpace(request.CollectionName))
            {
                return BadRequest("User input and collection name are required.");
            }

            var response = await _ragChatService.GenerateWithRag(request.UserInput, request.CollectionName);

            if (response.StartsWith("Input Error") || response.StartsWith("Processing Error"))
            {
                return BadRequest(response);
            }

            return Ok(new { response });
        }
    }
}

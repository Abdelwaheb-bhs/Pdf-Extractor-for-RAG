using Microsoft.AspNetCore.Mvc;
using RAGProject.Services.Interfaces;
using Nexia.Qdrant;
namespace RAGProject.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;


        public DocumentController(IEmbeddingService embeddingService, IQdrantService qdrantService)
        {
            _embeddingService = embeddingService;
            _qdrantService = qdrantService;


        }

        [HttpPost("save-document")]
        public async Task<IActionResult> GetText(IFormFile file, string category, string collectionName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();
            var result = await _embeddingService.SaveInformationAsync(collectionName, fileContent, id: file.FileName, description: category);

            if (!string.IsNullOrEmpty(result))
            {
                return Ok(new { message = result });
            }
            return  BadRequest("Failed to save document");   

          
            //var responseDtoList = vectors.Select(v => VectorResponseDto.FromVector(v)).ToList();

            //// Map to DTO and return response
            //return Ok(responseDtoList);

        }
        [HttpGet("list-collections")]
        public async Task<IActionResult> ListCollections()
        {
            try
            {
                // Fetch collections from the service
                var collections = new List<string>();
                await foreach (var collection in _qdrantService.GetCollectionsAsync())
                {
                    collections.Add(collection);
                }

                // Return the collections as a JSON response
                return Ok(new { Collections = collections });
            }
            catch (HttpRequestException httpEx)
            {
                // Log the exception details (can use logging framework here)
                return StatusCode(500, $"Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception details (can use logging framework here)
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpDelete("delete-by-filename")]
        public async Task<IActionResult> DeleteByFilename([FromQuery] string collectionName, [FromQuery] string filename)
        {
            await _qdrantService.RemoveAsync(collectionName, filename);
            return Ok(new { message = "Deletion successful" });
        }
    }
}

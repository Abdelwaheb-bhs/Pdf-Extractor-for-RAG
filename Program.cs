using RAGProject.Services.Impl;
using RAGProject.Services.Interfaces;
using Nexia.Qdrant;
using Nexia.Embeddings;
using Nexia.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IQdrantRepository>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var vectorSize = 128; // Set the appropriate vector size
    var endpoint = "http://localhost:6333"; // Set the appropriate endpoint
    var loggerFactory = sp.GetService<ILoggerFactory>();
    return new QdrantRepository(httpClient, vectorSize, endpoint, loggerFactory);
});

// Register services and dependencies
// Document processor service
builder.Services.AddSingleton<IDocumentProcessor, DocumentProcessor>();
//qdrant repository

//register qdrant service
builder.Services.AddSingleton<IQdrantService, QdrantService>();
//register embedding generation service
builder.Services.AddSingleton<ITextEmbeddingGenerationService, OnnxEmbeddingGeneration>();

//embedding service
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>(sp =>
    new EmbeddingService(
        
        sp.GetRequiredService<IQdrantService>(),
        sp.GetRequiredService<ITextEmbeddingGenerationService>()  
    ));


// Register MistralService
builder.Services.AddHttpClient<IMistralService, MistralService>();

//RAG service
builder.Services.AddSingleton<IRagChatService, RagChatService>(sp =>
    new RagChatService(
        sp.GetRequiredService<IQdrantRepository>(),
        sp.GetRequiredService<IEmbeddingService>(),
        sp.GetRequiredService<IMistralService>()
    ));


// Add controllers and API documentation (Swagger)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure authorization middleware if needed
app.UseAuthorization();

app.MapControllers();

app.Run();

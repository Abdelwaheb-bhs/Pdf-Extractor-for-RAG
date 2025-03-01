using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics.Tensors;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastBertTokenizer;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Nexia.Diagnostic;


namespace Nexia.Embeddings;
public class OnnxEmbeddingGeneration : ITextEmbeddingGenerationService, IDisposable
{
    private static readonly RunOptions s_runOptions = new();
    private static readonly string[] s_inputNames = ["input_ids", "attention_mask", "token_type_ids"];

    private readonly InferenceSession _onnxSession;
    private readonly BertTokenizer _tokenizer;
    private readonly int _dimensions;
    private readonly long[] _tokenTypeIds;
    private readonly int _maxTokens;
    public IReadOnlyDictionary<string, object> Attributes { get; } = new Dictionary<string, object>
{
    { "Model", "Qdrant/all-MiniLM-L6-v2-onnx" },
    { "Dimensions", 384 }
};
    public OnnxEmbeddingGeneration(string modelPath, string vocabPath, int maxTokens = 512)
    {
        _onnxSession = new InferenceSession(modelPath);
        _tokenizer = new BertTokenizer();
        using (var reader = new StreamReader(vocabPath))
        {
            _tokenizer.LoadVocabulary(reader, convertInputToLowercase: true);
        }
        _dimensions = _onnxSession.OutputMetadata.First().Value.Dimensions.Last();
        _maxTokens = maxTokens;
        _tokenTypeIds = new long[maxTokens];
    }

    public static OnnxEmbeddingGeneration Create(string onnxModelPath, string vocabPath)
    {
        Task<OnnxEmbeddingGeneration> t = CreateAsync(onnxModelPath, vocabPath, async: false, cancellationToken: default);
        Debug.Assert(t.IsCompleted);
        return t.GetAwaiter().GetResult();
    }

    public static async Task<OnnxEmbeddingGeneration> CreateAsync(
        string onnxModelPath,
        string vocabPath,
        bool async = true,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(onnxModelPath);
        Verify.NotNullOrWhiteSpace(vocabPath);

        using Stream onnxModelStream = new FileStream(onnxModelPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, async);
        using Stream vocabStream = new FileStream(vocabPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, async);

        return new OnnxEmbeddingGeneration(onnxModelPath, vocabPath);
    }

    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        if (data == null || data.Count == 0)
            return Array.Empty<ReadOnlyMemory<float>>();

        var results = new ReadOnlyMemory<float>[data.Count];
        var shape = new long[] { 1L, 0 };
        var inputValues = new OrtValue[3];
        var scratch = ArrayPool<long>.Shared.Rent(_maxTokens * 2);

        try
        {
            for (int i = 0; i < data.Count; i++)
            {
                string text = data[i];
                int tokenCount = _tokenizer.Encode(text, scratch.AsSpan(0, _maxTokens), scratch.AsSpan(_maxTokens, _maxTokens));
                shape[1] = tokenCount;

                using OrtValue inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, scratch.AsMemory(0, tokenCount), shape);
                using OrtValue attMaskOrtValue = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, scratch.AsMemory(_maxTokens, tokenCount), shape);
                using OrtValue typeIdsOrtValue = OrtValue.CreateTensorValueFromMemory(OrtMemoryInfo.DefaultInstance, _tokenTypeIds.AsMemory(0, tokenCount), shape);

                inputValues[0] = inputIdsOrtValue;
                inputValues[1] = attMaskOrtValue;
                inputValues[2] = typeIdsOrtValue;

                using var outputs = _onnxSession.Run(s_runOptions, s_inputNames, inputValues, _onnxSession.OutputNames);
                ReadOnlySpan<float> embeddingSpan = outputs[0].GetTensorDataAsSpan<float>();

                float[] pooledEmbedding = Pool(embeddingSpan);
                results[i] = new ReadOnlyMemory<float>(pooledEmbedding);
            }

            return results;
        }
        finally
        {
            ArrayPool<long>.Shared.Return(scratch);
        }
    }

    private float[] Pool(ReadOnlySpan<float> modelOutput)
    {
        if (modelOutput.Length == _dimensions)
        {
            float[] result = new float[_dimensions];
            modelOutput.CopyTo(result);
            return result;
        }

        int embeddings = modelOutput.Length / _dimensions;
        float[] pooled = new float[_dimensions];
        for (int pos = 0; pos < modelOutput.Length; pos += _dimensions)
        {
            TensorPrimitives.Add(pooled, modelOutput.Slice(pos, _dimensions), pooled);
        }
        TensorPrimitives.Divide(pooled, embeddings, pooled);
        return pooled;
    }

    public void Dispose()
    {
        _onnxSession.Dispose();
    }
}

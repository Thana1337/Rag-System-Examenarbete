// src/QueryService/Services/OllamaEmbedder.cs

namespace QueryService.Services
{
    internal interface IOllamaEmbedder
    {
        Task<float[]> EmbedAsync(string text, CancellationToken ct = default);
    }
}

namespace EmbeddingService.Services
{
    internal interface IOllamaEmbedder
    {
        Task<float[]> EmbedAsync(string text, CancellationToken ct = default);
    }
}

namespace EmbeddingService.Services
{
    internal interface IChunkingService
    {
        List<string> ChunkText(string text);
    }
}
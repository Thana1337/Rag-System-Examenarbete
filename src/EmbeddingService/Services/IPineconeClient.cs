
namespace EmbeddingService.Services
{
    internal interface IPineconeClient
    {
        Task UpsertAsync(string id, float[] values, Dictionary<string, object> metadata);
    }
}
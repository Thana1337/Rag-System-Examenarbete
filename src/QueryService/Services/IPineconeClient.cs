// src/QueryService/Services/PineconeClient.cs
namespace QueryService.Services
{
    public interface IPineconeClient
    {
        Task<List<PineconeMatch>> QueryAsync(
            float[] vector,
            int topK,
            string @namespace);
    }

    public record PineconeMatch(string Id, Dictionary<string, object> Metadata);
}
// src/QueryService/Services/OllamaCompletionService.cs

namespace QueryService.Services
{
    internal interface IOllamaCompletionService
    {
        Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
    }
}
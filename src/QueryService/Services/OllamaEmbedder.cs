// src/QueryService/Services/OllamaEmbedder.cs
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace QueryService.Services
{
    internal class OllamaEmbedder : IOllamaEmbedder
    {
        private readonly HttpClient _http;
        private readonly OllamaSettings _cfg;
        private readonly ILogger<OllamaEmbedder> _logger;

        public OllamaEmbedder(
            HttpClient http,
            IOptions<OllamaSettings> opts,
            ILogger<OllamaEmbedder> logger)
        {
            _http = http;
            _cfg = opts.Value;
            _logger = logger;
        }

        public async Task<float[]> EmbedAsync(
            string text, CancellationToken ct = default)
        {
            var req = new { model = _cfg.EmbedModel, prompt = text };
            _logger.LogInformation("Embedding question len={Len}", text.Length);

            using var resp = await _http.PostAsJsonAsync("/api/embeddings", req, ct);
            resp.EnsureSuccessStatusCode();
            var raw = await resp.Content.ReadAsStringAsync(ct);

            var wrapper = JsonSerializer.Deserialize<
                EmbeddingResponse>(raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            return wrapper.embedding;
        }

        private class EmbeddingResponse
        {
            public float[] embedding { get; set; } = Array.Empty<float>();
        }
    }
}
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Common;

namespace EmbeddingService.Services
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

        public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
        {
            var req = new
            {
                model = _cfg.EmbedModel,
                prompt = text
            };

            _logger.LogInformation("Embedding request → model={Model}, len={Len}",
                _cfg.EmbedModel, text.Length);

            using var resp = await _http.PostAsJsonAsync("/api/embeddings", req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP {Status} from Ollama: {Body}",
                    (int)resp.StatusCode, raw);
                resp.EnsureSuccessStatusCode();
            }

            EmbeddingResponse? wrapper;
            try
            {
                wrapper = JsonSerializer.Deserialize<EmbeddingResponse>(raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Ollama JSON: {Raw}", raw);
                throw;
            }

            if (wrapper?.embedding?.Length > 0)
            {
                _logger.LogInformation("Received vector dim={Dim}", wrapper.embedding.Length);
                return wrapper.embedding;
            }
            else
            {
                _logger.LogError("Empty embedding array in response: {Raw}", raw);
                throw new InvalidOperationException("Ollama returned no embedding");
            }
        }

        private class EmbeddingResponse
        {
            public float[]? embedding { get; set; }
        }
    }
}


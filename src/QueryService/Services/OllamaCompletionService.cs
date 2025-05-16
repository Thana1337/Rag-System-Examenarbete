// src/QueryService/Services/OllamaCompletionService.cs
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
    internal class OllamaCompletionService : IOllamaCompletionService
    {
        private readonly HttpClient _http;
        private readonly OllamaSettings _cfg;
        private readonly ILogger<OllamaCompletionService> _logger;

        public OllamaCompletionService(
            HttpClient http,
            IOptions<OllamaSettings> opts,
            ILogger<OllamaCompletionService> logger)
        {
            _http = http;
            _cfg = opts.Value;
            _logger = logger;
        }

        public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
        {
            var req = new
            {
                model = _cfg.CompletionModel,
                prompt = prompt,
                max_tokens = _cfg.MaxTokens,
                temperature = _cfg.Temperature,
                stream = false
            };

            _logger.LogInformation("Ollama generate → model={Model}, prompt-len={Len}",
                _cfg.CompletionModel, prompt.Length);

            using var resp = await _http.PostAsJsonAsync("/api/generate", req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Ollama HTTP {Status}: {Body}", (int)resp.StatusCode, raw);
                resp.EnsureSuccessStatusCode();
            }

            CompletionResponse? data;
            try
            {
                data = JsonSerializer.Deserialize<CompletionResponse>(raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Ollama JSON: {Raw}", raw);
                throw;
            }

            if (string.IsNullOrWhiteSpace(data?.response))
            {
                _logger.LogError("Empty response field from Ollama: {Raw}", raw);
                throw new InvalidOperationException("Ollama returned no response");
            }

            _logger.LogInformation("Ollama returned {Len} chars", data.response.Length);
            return data.response;
        }

        private class CompletionResponse
        {
            public string response { get; set; } = string.Empty;
        }
    }
}

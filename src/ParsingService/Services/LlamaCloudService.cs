using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ParsingService.Settings;

namespace ParsingService.Services
{
    internal class LlamaCloudService : ILlamaCloudService
    {
        private readonly HttpClient _http;
        private readonly LlamaCloudSettings _settings;
        private readonly ILogger<LlamaCloudService> _logger;

        public LlamaCloudService(
            HttpClient http,
            IOptions<LlamaCloudSettings> opts,
            ILogger<LlamaCloudService> logger)
        {
            _http = http;
            _settings = opts.Value;
            _logger = logger;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<string> ParsePdfAsync(Stream pdfStream, string fileName, CancellationToken ct = default)
        {
            // Upload
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(pdfStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "file", fileName);

            _logger.LogInformation("Uploading {FileName}", fileName);
            var uploadResp = await _http.PostAsync(_settings.UploadEndpoint, content, ct);
            uploadResp.EnsureSuccessStatusCode();
            var upJson = await uploadResp.Content.ReadAsStringAsync(ct);
            var upObj = JsonSerializer.Deserialize<UploadResponse>(upJson)!;
            _logger.LogInformation("Job ID {JobId}", upObj.Job_id);

            // Poll
            string status = "";
            for (int i = 0; i < _settings.MaxPolls; i++)
            {
                await Task.Delay(_settings.PollIntervalMs, ct);
                var stResp = await _http.GetAsync($"{_settings.StatusEndpoint}/{upObj.Job_id}", ct);
                stResp.EnsureSuccessStatusCode();
                var stJson = await stResp.Content.ReadAsStringAsync(ct);
                var stObj = JsonSerializer.Deserialize<JobStatusResponse>(stJson)!;
                status = stObj.Status ?? "";
                _logger.LogInformation("Poll {Attempt}/{Max}: {Status}", i + 1, _settings.MaxPolls, status);
                if (status.Equals("done", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    break;
                if (status.Equals("error", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Job {upObj.Job_id} failed");
            }

            // Fetch markdown
            var resultResp = await _http.GetAsync($"{_settings.ResultEndpoint}/{upObj.Job_id}/result/markdown", ct);
            resultResp.EnsureSuccessStatusCode();
            var markdown = await resultResp.Content.ReadAsStringAsync(ct);
            _logger.LogInformation("Markdown length {Len}", markdown.Length);
            return markdown;
        }

        private class UploadResponse
        {
            [JsonPropertyName("id")]
            public string? Job_id { get; set; }
        }

        private class JobStatusResponse
        {
            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }
    }
}

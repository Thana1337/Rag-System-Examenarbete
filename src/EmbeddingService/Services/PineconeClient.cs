using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EmbeddingService.Settings;

namespace EmbeddingService.Services
{
    internal class PineconeClient : IPineconeClient
    {
        private readonly HttpClient _http;
        private readonly PineconeSettings _settings;

        public PineconeClient(HttpClient http, IOptions<PineconeSettings> opts)
        {
            _http = http;
            _settings = opts.Value;
        }

        public async Task UpsertAsync(string id, float[] values, Dictionary<string, object> metadata)
        {
            var body = new
            {
                @namespace = "vectors",
                vectors = new[]
                {
                    new { id, values, metadata }
                }
            };
            var resp = await _http.PostAsJsonAsync("/vectors/upsert", body);
            resp.EnsureSuccessStatusCode();
        }
    }
}

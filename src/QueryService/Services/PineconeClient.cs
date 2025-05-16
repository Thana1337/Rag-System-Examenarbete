// src/QueryService/Services/PineconeClient.cs
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Options;

namespace QueryService.Services
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

        public async Task<List<PineconeMatch>> QueryAsync(
            float[] vector,
            int topK,
            string @namespace)
        {
            var body = new
            {
                vector,
                topK,
                includeMetadata = true,
                @namespace = @namespace ?? _settings.Namespace
            };

            var resp = await _http.PostAsJsonAsync("/query", body);
            resp.EnsureSuccessStatusCode();

            var wrapper = await resp.Content.ReadFromJsonAsync<QueryResponse>();

            return wrapper!.matches
                .Select(m => new PineconeMatch(m.id, m.metadata!))
                .ToList();
        }

        private class QueryResponse
        {
            public List<Match> matches { get; set; } = new();
            public class Match
            {
                public string id { get; set; } = string.Empty;
                public Dictionary<string, object>? metadata { get; set; }
            }
        }
    }
}
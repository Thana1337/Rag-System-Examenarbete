// src/QueryService/Services/QueryService.cs
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Options;
using QueryService.Models;

namespace QueryService.Services
{
    internal class QueryService : IQueryService
    {
        private readonly IOllamaEmbedder _embedder;
        private readonly IPineconeClient _pinecone;
        private readonly IOllamaCompletionService _completion;
        private readonly string _namespace;
        private readonly ILogger _logger;

        public QueryService(
            IOllamaEmbedder embedder,
            IPineconeClient pinecone,
            IOllamaCompletionService completion,
            ILogger<QueryService> logger,
            IOptions<PineconeSettings> pineOpts)
        {
            _embedder = embedder;
            _pinecone = pinecone;
            _completion = completion;
            _namespace = pineOpts.Value.Namespace;
            _logger = logger;
        }

        public async Task<QueryResult> AskAsync(
            string question, CancellationToken ct = default)
        {
            //Embed question
            var qVec = await _embedder.EmbedAsync(question, ct);

            //Retrieve top-K chunks
            var matches = await _pinecone.QueryAsync(qVec, topK: 5, @namespace: _namespace);

            //Build prompt from chunk texts
            var sb = new StringBuilder()
                .AppendLine("Use the following context to answer:");

            foreach (var match in matches)
            {
                if (match.Metadata.TryGetValue("text", out var raw))
                {
                    string chunkText;
                    switch (raw)
                    {
                        case string s:
                            chunkText = s;
                            break;
                        case JsonElement je
                            when je.ValueKind == JsonValueKind.String:
                            chunkText = je.GetString()!;
                            break;
                        default:
                            chunkText = raw?.ToString() ?? "";
                            break;
                    }

                    sb.AppendLine(chunkText)
                      .AppendLine("----");
                }
                else
                {
                    _logger.LogWarning("Match {Id} has no text metadata", match.Id);
                }
            }

            sb.AppendLine($"Question: {question}")
              .AppendLine("Answer:");

            var prompt = sb.ToString();

            //call LLM 
            var answer = await _completion.GenerateAsync(prompt, ct);

            //Return answer + sources
            return new QueryResult(
                answer,
                matches.Select(m => m.Id).ToList());
        }
    }
}
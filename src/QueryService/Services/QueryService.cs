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

            var sb = new StringBuilder();

          
            sb.AppendLine("You are a helpful AI assistant.  ")
              .AppendLine("Always format your answers to be as clear and scannable as possible:")
              .AppendLine("- Use markdown headings (e.g. `#`, `##`) for main sections")
              .AppendLine("- Use bullet points (`-`) for lists of items or key points")
              .AppendLine("- Use numbered steps (`1.`, `2.`, …) for any procedural instructions")
              .AppendLine("- Use **bold** or *italic* for emphasis")
              .AppendLine();


            foreach (var match in matches)
            {
                if (match.Metadata.TryGetValue("text", out var raw))
                {
                    string chunkText = raw switch
                    {
                        string s => s,
                        JsonElement je when je.ValueKind == JsonValueKind.String
                            => je.GetString()!,
                        _ => raw?.ToString() ?? ""
                    };

                    sb.AppendLine(chunkText.Trim())
                      .AppendLine("---");
                }
            }

            
            sb.AppendLine($"**Question:** {question}")
              .AppendLine()
              .AppendLine("**Answer:**")
              .AppendLine();

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
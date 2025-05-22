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

            var relevant = matches
            .Where(m => m.Metadata.ContainsKey("text"))
            .ToList();

            // om inget finns → returnera direkt
            if (!relevant.Any())
            {
                const string noInfoMsg =
                    "Sorry, I couldn’t find any relevant information in the provided documents.";
                return new QueryResult(noInfoMsg, new List<string>());
            }

            //Build prompt from chunk texts

            var sb = new StringBuilder()

                .AppendLine("You are a helpful AI assistant.")
                .AppendLine("Only answer questions that can be answered using the provided context.")
                .AppendLine("If the answer is not in the context, reply exactly:")
                .AppendLine("> Sorry, I couldn’t find any relevant information.")
                .AppendLine("Format your answer clearly using markdown headings, bullet points, or numbered steps.")
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
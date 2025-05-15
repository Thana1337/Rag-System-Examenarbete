using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace EmbeddingService.Services
{
    internal class EmbeddingService : IEmbeddingService
    {
        private readonly IPineconeClient _pinecone;
        private readonly IChunkingService _chunker;
        private readonly ILogger<EmbeddingService> _logger;

        public EmbeddingService(
            IPineconeClient pinecone,
            IChunkingService chunker,
            ILogger<EmbeddingService> logger)
        {
            _pinecone = pinecone;
            _chunker = chunker;
            _logger = logger;
        }

        public async Task ProcessAsync(DocumentParsedEvent evt, CancellationToken ct)
        {
            var text = evt.Markdown ?? "";
            var chunks = _chunker.ChunkText(text);

            _logger.LogInformation("Document {Id} → {Count} chunks",
                evt.DocumentId, chunks.Count);

            // Optional: inspect chunks locally
            var folder = Path.Combine(Path.GetTempPath(), "rag_chunks", evt.DocumentId);
            Directory.CreateDirectory(folder);
            for (int i = 0; i < chunks.Count; i++)
                await File.WriteAllTextAsync(
                    Path.Combine(folder, $"chunk_{i}.txt"),
                    chunks[i], ct);

            // TODO: replace dummy embedding with real embedder call
            for (int i = 0; i < chunks.Count; i++)
            {
                // e.g. var vector = await _embedder.EmbedAsync(chunks[i]);
                float[] vector = Enumerable.Repeat(0.5f, 768).ToArray();

                var metadata = new Dictionary<string, object>
                {
                    ["docId"] = evt.DocumentId,
                    ["chunkIndex"] = i
                };
                await _pinecone.UpsertAsync($"{evt.DocumentId}_c{i}", vector, metadata);
            }
        }
    }
}

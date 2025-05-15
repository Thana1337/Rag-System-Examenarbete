using System;
using System.Collections.Generic;
using System.Linq;

namespace EmbeddingService.Services
{
    internal class ChunkingService : IChunkingService
    {
        private const int MaxWords = 200;
        private const int OverlapWords = 50;
        private const int MinWords = 20;

        public List<string> ChunkText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();
            var words = text.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);

            var step = MaxWords - OverlapWords;
            var chunks = new List<string>();

            for (int start = 0; start < words.Length; start += step)
            {
                var length = Math.Min(MaxWords, words.Length - start);
                if (length < MinWords)
                    break;

                var slice = words.Skip(start).Take(length).ToArray();
                chunks.Add(string.Join(' ', slice));
            }

            return chunks
                .Distinct()
                .ToList();
        }
    }
}

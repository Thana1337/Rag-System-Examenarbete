using System.Threading;
using System.Threading.Tasks;
using Common.Models;

namespace EmbeddingService.Services
{

    public interface IEmbeddingService
    {
        Task ProcessAsync(DocumentParsedEvent evt, CancellationToken ct);
    }
}

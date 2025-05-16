// src/QueryService/Services/IQueryService.cs
using System.Threading;
using System.Threading.Tasks;
using QueryService.Models;

namespace QueryService.Services
{
    public interface IQueryService
    {
        Task<QueryResult> AskAsync(string question,
            CancellationToken ct = default);
    }
}
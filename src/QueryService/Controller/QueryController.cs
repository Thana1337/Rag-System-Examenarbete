// src/QueryService/Controllers/QueryController.cs
using Microsoft.AspNetCore.Mvc;
using QueryService.Models;
using QueryService.Services;

namespace QueryService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly IQueryService _svc;
        public QueryController(IQueryService svc) => _svc = svc;

        [HttpPost]
        public async Task<QueryResult> Post(QueryRequest req) =>
            await _svc.AskAsync(req.Question);
    }
}

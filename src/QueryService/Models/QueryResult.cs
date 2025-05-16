namespace QueryService.Models
{
    public record QueryResult(string Answer, List<string> SourceChunkIds);
}
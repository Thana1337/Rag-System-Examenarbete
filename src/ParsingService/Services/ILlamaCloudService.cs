namespace ParsingService.Services
{
    public interface ILlamaCloudService
    {
        Task<string> ParsePdfAsync(Stream pdfStream, string fileName, CancellationToken ct = default);
    }
}
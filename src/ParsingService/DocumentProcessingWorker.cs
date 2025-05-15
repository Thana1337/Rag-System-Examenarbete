// src/ParsingService/DocumentProcessingWorker.cs
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Common;
using Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ParsingService.Services;

namespace ParsingService
{
    public class DocumentProcessingWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly ServiceBusSender _parsedSender;
        private readonly BlobServiceClient _blobService;
        private readonly AzureStorageSettings _storageSettings;
        private readonly ILlamaCloudService _parser;
        private readonly ILogger<DocumentProcessingWorker> _logger;

        public DocumentProcessingWorker(
            ServiceBusProcessor processor,
            ServiceBusSender parsedSender,
            BlobServiceClient blobService,
            IOptions<AzureStorageSettings> storageOpts,
            ILlamaCloudService parser,
            ILogger<DocumentProcessingWorker> logger)
        {
            _processor = processor;
            _parsedSender = parsedSender;
            _blobService = blobService;
            _storageSettings = storageOpts.Value;
            _parser = parser;
            _logger = logger;

            _processor.ProcessMessageAsync += OnMessageReceivedAsync;
            _processor.ProcessErrorAsync += OnErrorAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting processor for queue '{Queue}'", _processor.EntityPath);
            await _processor.StartProcessingAsync(stoppingToken);
            try { await Task.Delay(Timeout.Infinite, stoppingToken); }
            catch (TaskCanceledException) { }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping processor...");
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task OnMessageReceivedAsync(ProcessMessageEventArgs args)
        {
            string? documentId = null;
            try
            {
                // 1) Deserialize incoming upload event
                var raw = args.Message.Body.ToArray();
                var uploadEvt = JsonSerializer.Deserialize<DocumentUploadedEvent>(
                    raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (uploadEvt is null)
                {
                    _logger.LogWarning("Received invalid upload event, completing.");
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                    return;
                }

                documentId = uploadEvt.DocumentId;
                _logger.LogInformation("Processing document {DocumentId}", documentId);

                // 2) Download PDF from Blob
                var container = _blobService.GetBlobContainerClient(_storageSettings.ContainerName);
                var ext = Path.GetExtension(uploadEvt.FileName) ?? ".pdf";
                var blobClient = container.GetBlobClient($"{documentId}{ext}");

                var tmpDir = Path.Combine(Path.GetTempPath(), "rag_downloads");
                Directory.CreateDirectory(tmpDir);
                var localPath = Path.Combine(tmpDir, $"{documentId}{ext}");

                try
                {
                    await blobClient.DownloadToAsync(localPath, args.CancellationToken);
                    _logger.LogInformation("Downloaded to {Path}", localPath);

                    // 3) Parse PDF to Markdown
                    await using var fs = File.OpenRead(localPath);
                    var markdown = await _parser.ParsePdfAsync(fs, uploadEvt.FileName, args.CancellationToken);
                    _logger.LogInformation("Parsed markdown length: {Len}", markdown.Length);

                    // 4) Publish parsed event
                    var parsedEvt = new DocumentParsedEvent
                    {
                        DocumentId = documentId,
                        Markdown = markdown
                    };
                    var msg = new ServiceBusMessage(JsonSerializer.Serialize(parsedEvt))
                    {
                        ContentType = "application/json"
                    };
                    await _parsedSender.SendMessageAsync(msg, args.CancellationToken);
                    _logger.LogInformation("Published DocumentParsedEvent for {DocumentId}", documentId);
                }
                finally
                {
                    if (File.Exists(localPath))
                        File.Delete(localPath);
                }

                // 5) Complete original message
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document {DocumentId}", documentId);
                await args.DeadLetterMessageAsync(args.Message, cancellationToken: args.CancellationToken);
            }
        }

        private Task OnErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus error on {Path}", args.EntityPath);
            return Task.CompletedTask;
        }
    }
}

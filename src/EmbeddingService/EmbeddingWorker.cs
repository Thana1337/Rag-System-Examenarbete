// src/EmbeddingService/EmbeddingWorker.cs
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Common.Models;
using EmbeddingService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmbeddingService
{
    public class EmbeddingWorker : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IEmbeddingService _service;
        private readonly ILogger<EmbeddingWorker> _logger;

        public EmbeddingWorker(
            ServiceBusProcessor processor,
            IEmbeddingService service,
            ILogger<EmbeddingWorker> logger)
        {
            _processor = processor;
            _service = service;
            _logger = logger;

            _processor.ProcessMessageAsync += OnMessageAsync;
            _processor.ProcessErrorAsync += OnErrorAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting embedding processor on {Queue}", _processor.EntityPath);
            await _processor.StartProcessingAsync(stoppingToken);
            try { await Task.Delay(Timeout.Infinite, stoppingToken); }
            catch (TaskCanceledException) { }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping embedding processor...");
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

        private async Task OnMessageAsync(ProcessMessageEventArgs args)
        {
            DocumentParsedEvent evt;
            try
            {
                evt = JsonSerializer.Deserialize<DocumentParsedEvent>(
                          args.Message.Body.ToArray(),
                          new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                      )
                      ?? throw new JsonException("Unable to deserialize DocumentParsedEvent");

                _logger.LogInformation("Embedding document {Id}", evt.DocumentId);
                await _service.ProcessAsync(evt, args.CancellationToken);
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error embedding, dead-lettering message");
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

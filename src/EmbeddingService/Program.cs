using System;
using System.Net.Http.Headers;
using Azure.Messaging.ServiceBus;
using Common;
using EmbeddingService;
using EmbeddingService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

// Configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>(optional: true);

// Bind shared ServiceBus settings
builder.Services.Configure<ServiceBusSettings>(
    builder.Configuration.GetSection("AzureServiceBus"));

// Bind Pinecone settings
builder.Services.Configure<PineconeSettings>(
    builder.Configuration.GetSection("PineconeSettings"));


// ServiceBusProcessor for "documentparsed"
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    var client = new ServiceBusClient(cfg.ConnectionString);
    var opts = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = 4,
        PrefetchCount = 10
    };
    return client.CreateProcessor(cfg.QueueName, opts);
});

// Pinecone HTTP client
builder.Services.AddHttpClient<IPineconeClient, PineconeClient>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IOptions<PineconeSettings>>().Value;
    http.BaseAddress = new Uri(cfg.Host);
    http.DefaultRequestHeaders.Add("Api-Key", cfg.ApiKey);
    http.DefaultRequestHeaders.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Ollama embedder
builder.Services.Configure<OllamaSettings>(
    builder.Configuration.GetSection("OllamaSettings"));

builder.Services.AddHttpClient<IOllamaEmbedder, OllamaEmbedder>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;
    var baseUrl = cfg.BaseUrl?.Trim() ?? "";

    // If no scheme, assume http
    if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
        !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
    {
        baseUrl = "http://" + baseUrl;
    }

    http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
    http.DefaultRequestHeaders.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Chunking service
builder.Services.AddSingleton<IChunkingService, ChunkingService>();

// Embedding service + worker
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService.Services.EmbeddingService>();
builder.Services.AddHostedService<EmbeddingWorker>();


await builder.Build().RunAsync();

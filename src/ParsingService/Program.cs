using System;
using System.Net.Http.Headers;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ParsingService;
using ParsingService.Services;
using ParsingService.Settings;

var builder = Host.CreateApplicationBuilder(args);

//Configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
    builder.Configuration.AddUserSecrets<Program>(optional: true);

//if (builder.Environment.IsDevelopment())
//{
//    builder.Configuration.AddUserSecrets<Program>(optional: true);
//}
//else
//{
//    var kvName = builder.Configuration["KeyVault:Name"];
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{kvName}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

//Bind settings
builder.Services.Configure<ServiceBusSettings>(
    builder.Configuration.GetSection("AzureServiceBus"));
builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection("AzureStorage"));
builder.Services.Configure<LlamaCloudSettings>(
    builder.Configuration.GetSection("LlamaCloud"));

//Blob client
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<AzureStorageSettings>>().Value;
    return new BlobServiceClient(cfg.ConnectionString);
});

//ServiceBusProcessor
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    var client = new ServiceBusClient(cfg.ConnectionString);
    var opts = new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = 5,
        PrefetchCount = 10
    };
    return client.CreateProcessor(cfg.QueueName, opts);
});

builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    var client = new ServiceBusClient(cfg.ConnectionString);
    return client.CreateSender("documentparsed");
});

//LlamaParse HTTP client
builder.Services.AddHttpClient<ILlamaCloudService, LlamaCloudService>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IOptions<LlamaCloudSettings>>().Value;
    http.BaseAddress = new Uri(cfg.BaseUrl);
    http.Timeout = TimeSpan.FromMinutes(5);
    http.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", cfg.ApiKey);
});

//Worker
builder.Services.AddHostedService<DocumentProcessingWorker>();

await builder.Build().RunAsync();

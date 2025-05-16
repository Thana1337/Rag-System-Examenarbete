using System;
using System.Net.Http.Headers;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QueryService.Services;

var builder = WebApplication.CreateBuilder(args);

//Configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<Program>(optional: true);

//Bind settings
builder.Services.Configure<PineconeSettings>(
    builder.Configuration.GetSection("PineconeSettings"));
builder.Services.Configure<OllamaSettings>(
    builder.Configuration.GetSection("OllamaSettings"));

//Pinecone HTTP client
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

builder.Services.AddHttpClient<IOllamaCompletionService, OllamaCompletionService>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;
    var baseUrl = cfg.BaseUrl?.Trim() ?? "";

    if (!baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
        !baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
    {
        baseUrl = "http://" + baseUrl;
    }

    http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
    http.Timeout = TimeSpan.FromMinutes(5);
    http.DefaultRequestHeaders.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddSingleton<IQueryService, QueryService.Services.QueryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opts =>
  opts.AddDefaultPolicy(pb =>
    pb.WithOrigins(CorsSettings.AllowedOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod()
  )
);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

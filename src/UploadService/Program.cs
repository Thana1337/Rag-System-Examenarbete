using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Common;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

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

//Azure Blob Storage
builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection("AzureStorage"));
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<AzureStorageSettings>>().Value;
    return new BlobServiceClient(cfg.ConnectionString);
});

//Azure Service Bus
builder.Services.Configure<ServiceBusSettings>(
    builder.Configuration.GetSection("AzureServiceBus"));
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    return new ServiceBusClient(cfg.ConnectionString);
});
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(cfg.QueueName);
});




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin()));

builder.Services.AddCors(opts =>
  opts.AddDefaultPolicy(pb =>
    pb.WithOrigins(CorsSettings.AllowedOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod()
  )
);

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();

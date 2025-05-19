using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class CorsSettings
    {
        public static readonly string[] AllowedOrigins =
        {
        "http://localhost:5173" 
    };
    }
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
    }
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }

    public class PineconeSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Namespace { get; set; } = string.Empty;
    }

    public class OllamaSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string EmbedModel { get; set; } = string.Empty;
        public string CompletionModel { get; set; } = string.Empty;
        public int MaxTokens { get; set; } 
        public double Temperature { get; set; } 
    }
}

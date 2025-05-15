using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingService.Settings
{
    public class LlamaCloudSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.cloud.llamaindex.ai";
        public string UploadEndpoint { get; set; } = "/api/v1/parsing/upload";
        public string StatusEndpoint { get; set; } = "/api/v1/parsing/job";
        public string ResultEndpoint { get; set; } = "/api/v1/parsing/job";
        public int MaxPolls { get; set; } = 20;
        public int PollIntervalMs { get; set; } = 3000;
    }
}

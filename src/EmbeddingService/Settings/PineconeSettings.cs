using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddingService.Settings
{
    public class PineconeSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class DocumentParsedEvent
    {
        public string DocumentId { get; set; } = string.Empty;
        public string Markdown { get; set; } = string.Empty;
    }
}

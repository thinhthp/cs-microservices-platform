using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.DTOs.Mail
{
    public sealed class EmailEnvelope
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Html { get; set; } = default!;
        public string Template { get; set; } = "RawHtml";
        public string? CorrelationId { get; set; }
        public string? MessageId { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
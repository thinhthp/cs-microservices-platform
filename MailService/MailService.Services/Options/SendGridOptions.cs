using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Services.Options
{
    public sealed class SendGridOptions
    {
        [Required] public string ApiKey { get; set; } = default!;
        [Required, EmailAddress] public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = "Shirli";
    }
}
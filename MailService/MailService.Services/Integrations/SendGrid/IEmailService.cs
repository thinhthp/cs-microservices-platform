using MailService.Services.DTOs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Services.Integrations.SendGrid
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailEnvelope emailEnvelope, CancellationToken ct = default);
    }
}
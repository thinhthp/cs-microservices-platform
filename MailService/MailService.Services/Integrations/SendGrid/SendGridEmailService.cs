using MailService.Services.DTOs.Mail;
using MailService.Services.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Services.Integrations.SendGrid
{
    public class SendGridEmailService : IEmailService
    {
        SendGridOptions _options;

        public SendGridEmailService(IOptions<SendGridOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(EmailEnvelope emailEnvelope, CancellationToken ct = default)
        {
            var client = new SendGridClient(_options.ApiKey);
            var from = new EmailAddress(_options.FromEmail, _options.FromName);
            var to = new EmailAddress(emailEnvelope.To);
            var msg = MailHelper.CreateSingleEmail(from, to, emailEnvelope.Subject, plainTextContent: null, emailEnvelope.Html);
            var response = await client.SendEmailAsync(msg);
        }
    }
}

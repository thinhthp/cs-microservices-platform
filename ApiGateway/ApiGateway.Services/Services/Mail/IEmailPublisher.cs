using ApiGateway.Services.DTOs.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.Services.Mail
{
    public interface IEmailPublisher
    {
        Task PublishAsync(EmailEnvelope message, CancellationToken ct = default);
    }
}
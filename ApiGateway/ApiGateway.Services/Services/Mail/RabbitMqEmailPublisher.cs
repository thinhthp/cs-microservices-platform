using ApiGateway.Services.Constants;
using ApiGateway.Services.DTOs.Mail;
using ApiGateway.Services.Messaging;
using ApiGateway.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace ApiGateway.Services.Services.Mail
{
    public sealed class RabbitMqEmailPublisher : IEmailPublisher
    {
        private readonly ILogger<RabbitMqEmailPublisher> _logger;
        private readonly RabbitMqConnectionManager _connectionManager;

        public RabbitMqEmailPublisher(RabbitMqConnectionManager rabbitMqConnectionManager, ILogger<RabbitMqEmailPublisher> logger)
        {
            _connectionManager = rabbitMqConnectionManager;
            _logger = logger;
        }

        public async Task PublishAsync(EmailEnvelope message, CancellationToken ct = default)
        {
            var conn = await _connectionManager.GetOrCreateConnectionAsync(ct).ConfigureAwait(false);

            await using var channel = await conn.CreateChannelAsync(cancellationToken: ct).ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(RabbitMqRouting.ExchangeEmail, ExchangeType.Topic, durable: true, autoDelete: false);

            var payload = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(payload);

            // 2 cases
            var routingKey = message.Template switch
            {
                "ConfirmEmail" => RabbitMqRouting.EmailConfirm,
                "PasswordReset" => RabbitMqRouting.EmailForgot,
                _ => RabbitMqRouting.EmailConfirm // fallback to confirm to avoid drops
            };

            var props = new BasicProperties
            {
                ContentType = "application/json",
                MessageId = message.MessageId ?? Guid.NewGuid().ToString("N"),
                CorrelationId = message.CorrelationId,
                Type = routingKey
            };
            props.DeliveryMode = (DeliveryModes)2; // persistent

            await channel.BasicPublishAsync(
                exchange: RabbitMqRouting.ExchangeEmail,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body
            );

            _logger.LogDebug("Published email to {Exchange} with key {Key} (MessageId={MessageId})",
                RabbitMqRouting.ExchangeEmail, routingKey, props.MessageId);
        }
    }
}
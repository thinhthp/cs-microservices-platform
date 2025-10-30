using MailService.Services.Constants;
using MailService.Services.DTOs.Mail;
using MailService.Services.Integrations.SendGrid;
using MailService.Services.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MailService.Services.Services.Mail
{
    public sealed class RabbitMqEmailConsumer : BackgroundService
    {
        RabbitMqConnectionManager _connectionManager;
        private readonly ILogger<RabbitMqEmailConsumer> _logger;
        private readonly IEmailService _emailSender;
        private string? _consumerTag;

        public RabbitMqEmailConsumer(RabbitMqConnectionManager connectionManager, ILogger<RabbitMqEmailConsumer> logger, IEmailService emailService)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _emailSender = emailService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conn = await _connectionManager.GetOrCreateConnectionAsync(stoppingToken).ConfigureAwait(false);
            await using var channel = await conn.CreateChannelAsync(cancellationToken: stoppingToken).ConfigureAwait(false);

            await channel.ExchangeDeclareAsync(RabbitMqRouting.ExchangeEmail, ExchangeType.Topic, durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(RabbitMqRouting.ExchangeEmail + ".queue", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(RabbitMqRouting.ExchangeEmail + ".queue", RabbitMqRouting.ExchangeEmail, "email.#");

            // limit
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken).ConfigureAwait(false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            // run that when it runs
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var envelope = JsonSerializer.Deserialize<EmailEnvelope>(json);
                    if (envelope is null) throw new InvalidOperationException("Envelope is null");

                    await _emailSender.SendEmailAsync(envelope, stoppingToken);

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed processing message {DeliveryTag}", ea.DeliveryTag);
                    // NACK without requeue (to be checked)
                    await channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            // Start consumer and store the tag for something idk
            _consumerTag = await channel.BasicConsumeAsync(queue: RabbitMqRouting.ExchangeEmail + ".queue", autoAck: false, consumer: consumer);

            // keep running until cancellation
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) { /* shutdown */ }
        }
    }
}

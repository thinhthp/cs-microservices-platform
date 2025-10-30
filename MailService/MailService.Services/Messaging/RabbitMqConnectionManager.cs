using MailService.Services.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Services.Messaging
{
    public sealed class RabbitMqConnectionManager : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private RabbitMQ.Client.IConnection? _connection;
        private readonly RabbitMqOptions _options;
        private readonly SemaphoreSlim _connLock = new(1, 1);

        public RabbitMqConnectionManager(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
            _factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
        }

        internal async Task<RabbitMQ.Client.IConnection> GetOrCreateConnectionAsync(CancellationToken ct)
        {
            if (_connection is { IsOpen: true }) return _connection;

            await _connLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_connection is { IsOpen: true }) return _connection;

                _connection = await _factory.CreateConnectionAsync(ct).ConfigureAwait(false);
                return _connection;
            }
            finally
            {
                _connLock.Release();
            }
        }

        public void Dispose()
        {
            try { _connection?.Dispose(); } catch { }
        }
    }
}

using Microsoft.Extensions.Logging;
using SalesService.DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork uow, ILogger<PaymentService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Entities.Entities.Payment> CreateAsync(Guid orderId, decimal amount, string method)
        {
            if (orderId == Guid.Empty) throw new ArgumentException("OrderId required.", nameof(orderId));
            if (amount <= 0) throw new ArgumentException("Amount must be > 0.", nameof(amount));
            if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("Method required.", nameof(method));

            var order = await _uow.Orders.GetByIdAsync(orderId, includeDetails: false);
            if (order is null) throw new KeyNotFoundException("Order not found.");

            var existing = await _uow.Payments.GetByOrderIdAsync(orderId);
            if (existing is not null) throw new InvalidOperationException("Payment already exists for this order.");

            var payment = new Entities.Entities.Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Amount = amount,
                Method = method,
                PaidAt = DateTimeOffset.UtcNow
            };

            await _uow.Payments.AddAsync(payment);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} created for Order {OrderId}.", payment.Id, orderId);
            return payment;
        }

        public Task<Entities.Entities.Payment?> GetByIdAsync(Guid id) => _uow.Payments.GetByIdAsync(id);

        public Task<Entities.Entities.Payment?> GetByOrderAsync(Guid orderId) => _uow.Payments.GetByOrderIdAsync(orderId);

        public Task<IReadOnlyList<Entities.Entities.Payment>> GetAllAsync() => _uow.Payments.GetAllAsync();
    }
}
using Microsoft.Extensions.Logging;
using SalesService.BLL.DTOs.Inventory;
using SalesService.BLL.Grpcs.Inventory;
using SalesService.BLL.Grpcs.Product;
using SalesService.DAL.Common;
using SalesService.Entities.Entities;
using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IInventoryGrpcClient _inventoryClient;
        private readonly IProductGrpcClient _productClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork uow,
            IInventoryGrpcClient inventoryClient,
            IProductGrpcClient productClient,
            ILogger<OrderService> logger)
        {
            _uow = uow;
            _inventoryClient = inventoryClient;
            _productClient = productClient;
            _logger = logger;
        }

        public async Task<Orders> CreateOrderAsync(Guid customerId, Guid dealerId, IEnumerable<OrderItemInput> items)
        {
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId required.", nameof(customerId));
            if (dealerId == Guid.Empty) throw new ArgumentException("DealerId required.", nameof(dealerId));

            var itemList = items?.ToList();
            if (itemList is null || itemList.Count == 0)
                throw new ArgumentException("At least one item required.", nameof(items));

            var customer = await _uow.Customers.GetByIdAsync(customerId)
                ?? throw new KeyNotFoundException("Customer not found.");

            GetDealerResponse? dealerGrpc;
            try
            {
                dealerGrpc = await _inventoryClient.GetDealerByIdAsync(dealerId);
            }
            catch (Grpc.Core.RpcException ex)
            {
                _logger.LogWarning("Inventory gRPC lookup failed. dealerId={DealerId} statusCode={StatusCode} detail={Detail}",
                    dealerId, ex.StatusCode, ex.Status.Detail);
                throw new KeyNotFoundException($"Dealer lookup failed in Inventory service. dealerId={dealerId} status={ex.StatusCode}");
            }

            if (dealerGrpc is null)
            {
                var msg = $"Dealer not found in Inventory service. dealerId={dealerId}";
                _logger.LogWarning(msg);
                throw new KeyNotFoundException(msg);
            }

            _logger.LogDebug("Validated dealer via gRPC. dealerId={DealerId}.", dealerId);

            var order = new Orders
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                DealerId = dealerId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };

            foreach (var input in itemList)
            {
                if (input.VariantId == Guid.Empty) throw new ArgumentException("VariantId required.");
                if (input.Quantity <= 0) throw new ArgumentException("Quantity must be > 0.");
                if (input.UnitPrice < 0) throw new ArgumentException("UnitPrice must be >= 0.");

                var variantGrpc = await _productClient.GetVariantById(input.VariantId);
                if (variantGrpc is null)
                    throw new KeyNotFoundException($"Variant {input.VariantId} not found in Product service.");

                var unitPrice = input.UnitPrice;
                if (unitPrice == 0 && variantGrpc.BasePrice.HasValue)
                {
                    unitPrice = (decimal)variantGrpc.BasePrice.Value;
                    _logger.LogDebug("Unit price for variant {VariantId} filled from gRPC basePrice={BasePrice}.",
                        input.VariantId, unitPrice);
                }

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    VariantId = input.VariantId,
                    Quantity = input.Quantity,
                    UnitPrice = unitPrice
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await _uow.Orders.AddAsync(order);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} created for customer {CustomerId} dealer {DealerId} items={ItemCount} total={Total}.",
                order.Id, customerId, dealerId, order.Items.Count, order.TotalAmount);

            return order;
        }

        public Task<Orders?> GetByIdAsync(Guid orderId, bool includeDetails = false)
            => _uow.Orders.GetByIdAsync(orderId, includeDetails);

        public Task<IReadOnlyList<Orders>> GetByCustomerAsync(Guid customerId, bool includeDetails = false)
            => _uow.Orders.GetByCustomerAsync(customerId, includeDetails);

        public async Task UpdateStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _uow.Orders.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of cancelled order.");

            await _uow.Orders.UpdateStatusAsync(orderId, status);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Order {OrderId} status changed to {Status}.", orderId, status);
        }

        public Task<IReadOnlyList<Orders>> GetAllAsync(bool includeDetails = false)
            => _uow.Orders.GetAllAsync(includeDetails);
    }
}
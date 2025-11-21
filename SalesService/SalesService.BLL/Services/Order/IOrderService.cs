using SalesService.Entities.Entities;
using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Order
{
    public interface IOrderService
    {
        Task<Orders> CreateOrderAsync(Guid customerId, Guid dealerId, IEnumerable<OrderItemInput> items);
        Task<Orders?> GetByIdAsync(Guid orderId, bool includeDetails = false);
        Task<IReadOnlyList<Orders>> GetByCustomerAsync(Guid customerId, bool includeDetails = false);
        Task UpdateStatusAsync(Guid orderId, OrderStatus status);
        Task<IReadOnlyList<Orders>> GetAllAsync(bool includeDetails = false);
    }

    public record OrderItemInput(Guid VariantId, int Quantity, decimal UnitPrice);
}

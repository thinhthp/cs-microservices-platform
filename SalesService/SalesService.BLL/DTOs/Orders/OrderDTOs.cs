using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Orders
{
    public record OrderDto(
            Guid Id,
            Guid CustomerId,
            Guid DealerId,
            OrderStatus Status,
            decimal TotalAmount,
            DateTimeOffset CreatedAt,
            List<OrderItemDto>? Items);

    public record OrderItemDto(
        Guid Id,
        Guid VariantId,
        int Quantity,
        decimal UnitPrice);
}


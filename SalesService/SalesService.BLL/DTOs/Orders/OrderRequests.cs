using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Orders
{
    public record CreateOrderRequest(
            [Required] Guid CustomerId,
            [Required] Guid DealerId,
            [Required, MinLength(1)] List<CreateOrderItemRequest> Items);

    public record CreateOrderItemRequest(
        [Required] Guid VariantId,
        [Range(1, int.MaxValue)] int Quantity,
        [Range(0, double.MaxValue)] decimal UnitPrice);

    public record UpdateOrderStatusRequest([Required] OrderStatus Status);
}

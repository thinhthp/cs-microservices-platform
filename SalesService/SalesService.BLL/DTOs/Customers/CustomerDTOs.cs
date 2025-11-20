using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Customers
{
    public record CustomerOrderSummary(
            Guid Id,
            decimal TotalAmount,
            SalesService.Entities.Enums.OrderStatus Status,
            DateTimeOffset CreatedAt);

    public record CustomerDto(
            Guid Id,
            string FullName,
            string? Phone,
            string? Email,
            List<CustomerOrderSummary>? Orders);
}

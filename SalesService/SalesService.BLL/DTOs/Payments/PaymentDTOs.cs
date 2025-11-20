using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Payments
{
    public record PaymentDto(
            Guid Id,
            Guid OrderId,
            decimal Amount,
            string Method,
            DateTimeOffset PaidAt);
}

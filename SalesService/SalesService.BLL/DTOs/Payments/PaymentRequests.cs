using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Payments
{
    public record CreatePaymentRequest(
            [Required] Guid OrderId,
            [Range(0.01, double.MaxValue)] decimal Amount,
            [Required, StringLength(64)] string Method);
}

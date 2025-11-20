using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.DTOs.Customers
{
    public record CreateCustomerRequest(
            [Required, StringLength(200)] string FullName,
            [StringLength(32)] string? Phone,
            [EmailAddress, StringLength(256)] string? Email);
}

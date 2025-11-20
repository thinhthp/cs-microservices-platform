using SalesService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Customers
{
    public interface ICustomerService
    {
        Task<Customer> CreateAsync(string fullName, string? phone, string? email);
        Task<Customer?> GetByIdAsync(Guid id, bool includeOrders = false);
        Task<IReadOnlyList<Customer>> SearchAsync(string? term);
    }
}
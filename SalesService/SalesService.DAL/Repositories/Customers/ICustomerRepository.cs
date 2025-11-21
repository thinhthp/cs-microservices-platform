using SalesService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Customers
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, bool includeOrders = false);
        Task AddAsync(Customer customer);
        Task<IReadOnlyList<Customer>> SearchAsync(string? term, int take = 50);
    }
}
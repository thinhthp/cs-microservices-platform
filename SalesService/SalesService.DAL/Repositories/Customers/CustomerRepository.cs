using Microsoft.EntityFrameworkCore;
using SalesService.DAL.Data;
using SalesService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SalesContext _salesContext;

        public CustomerRepository(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public async Task<Customer?> GetByIdAsync(Guid id, bool includeOrders = false)
        {
            IQueryable<Customer> query = _salesContext.Customers;
            if (includeOrders)
            {
                query = query.Include(c => c.Orders)
                             .ThenInclude(o => o.Items)
                             .Include(c => c.Orders)
                             .ThenInclude(o => o.Payment);
            }
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _salesContext.Customers.AddAsync(customer);
        }

        public async Task<IReadOnlyList<Customer>> SearchAsync(string? term, int take = 50)
        {
            term = term?.Trim();
            IQueryable<Customer> query = _salesContext.Customers.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(c =>
                    EF.Functions.ILike(c.FullName, $"%{term}%") ||
                    EF.Functions.ILike(c.Email, $"%{term}%"));
            }
            return await query.OrderBy(c => c.FullName)
                              .Take(take)
                              .ToListAsync();
        }
    }
}
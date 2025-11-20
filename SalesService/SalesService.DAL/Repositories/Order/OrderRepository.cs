using Microsoft.EntityFrameworkCore;
using SalesService.DAL.Data;
using SalesService.Entities.Entities;
using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SalesContext _salesContext;

        public OrderRepository(SalesContext salesContext) {
            _salesContext = salesContext;
        }

        public async Task<Orders?> GetByIdAsync(Guid id, bool includeDetails = false)
        {
            IQueryable<Orders> query = _salesContext.Orders;
            if (includeDetails)
            {
                query = query
                    .Include(o => o.Items)
                    .Include(o => o.Payment)
                    .Include(o => o.Customer);
            }
            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Orders order)
        {
            await _salesContext.Orders.AddAsync(order);
        }

        public async Task<IReadOnlyList<Orders>> GetByCustomerAsync(Guid customerId, bool includeDetails = false)
        {
            IQueryable<Orders> query = _salesContext.Orders.Where(o => o.CustomerId == customerId);
            if (includeDetails)
            {
                query = query
                    .Include(o => o.Items)
                    .Include(o => o.Payment);
            }
            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _salesContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return;
            order.Status = status;
        }

        public async Task<IReadOnlyList<Orders>> GetAllAsync(bool includeDetails = false)
        {
            IQueryable<Orders> query = _salesContext.Orders;
            if (includeDetails)
            {
                query = query
                    .Include(o => o.Items)
                    .Include(o => o.Payment)
                    .Include(o => o.Customer);
            }
            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
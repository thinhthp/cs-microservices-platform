using SalesService.Entities.Entities;
using SalesService.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Order
{
    public interface IOrderRepository
    {
        Task<Orders?> GetByIdAsync(Guid id, bool includeDetails = false);
        Task AddAsync(Orders order);
        Task<IReadOnlyList<Orders>> GetByCustomerAsync(Guid customerId, bool includeDetails = false);
        Task UpdateStatusAsync(Guid orderId, OrderStatus status);
    }
}
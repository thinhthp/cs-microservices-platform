using Microsoft.EntityFrameworkCore;
using SalesService.DAL.Data;
using SalesService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly SalesContext _salesContext;

        public PaymentRepository(SalesContext salesContext)
        {
            _salesContext = salesContext;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _salesContext.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        {
            return await _salesContext.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task AddAsync(Payment payment)
        {
            await _salesContext.Payments.AddAsync(payment);
        }

        public async Task<IReadOnlyList<Payment>> GetAllAsync()
        {
            return await _salesContext.Payments
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }
    }
}
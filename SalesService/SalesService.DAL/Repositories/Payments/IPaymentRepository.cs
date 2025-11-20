using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<SalesService.Entities.Entities.Payment?> GetByIdAsync(Guid id);
        Task<SalesService.Entities.Entities.Payment?> GetByOrderIdAsync(Guid orderId);
        Task AddAsync(SalesService.Entities.Entities.Payment payment);
        Task<IReadOnlyList<SalesService.Entities.Entities.Payment>> GetAllAsync();
    }
}

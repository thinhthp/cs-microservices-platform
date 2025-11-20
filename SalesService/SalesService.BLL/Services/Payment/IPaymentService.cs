using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Payment
{
    public interface IPaymentService
    {
        Task<SalesService.Entities.Entities.Payment> CreateAsync(Guid orderId, decimal amount, string method);
        Task<SalesService.Entities.Entities.Payment?> GetByIdAsync(Guid id);
        Task<SalesService.Entities.Entities.Payment?> GetByOrderAsync(Guid orderId);
    }
}

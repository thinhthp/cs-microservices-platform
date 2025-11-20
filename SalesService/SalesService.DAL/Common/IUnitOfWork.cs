using SalesService.DAL.Repositories.Customers;
using SalesService.DAL.Repositories.Order;
using SalesService.DAL.Repositories.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.DAL.Common
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }
        ICustomerRepository Customers { get; }
    }
}

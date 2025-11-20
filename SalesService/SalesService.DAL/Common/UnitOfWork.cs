using Microsoft.EntityFrameworkCore;
using SalesService.DAL.Data;
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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SalesContext _context;
        private IOrderRepository? _orders;
        private IPaymentRepository? _payments;
        private ICustomerRepository? _customers;

        public UnitOfWork(SalesContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
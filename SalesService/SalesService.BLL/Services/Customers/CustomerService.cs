using Microsoft.Extensions.Logging;
using SalesService.DAL.Common;
using SalesService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalesService.BLL.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork uow, ILogger<CustomerService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Customer> CreateAsync(string fullName, string? phone, string? email)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName is required.", nameof(fullName));

            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email format invalid.", nameof(email));

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FullName = fullName.Trim(),
                Phone = phone?.Trim(),
                Email = email?.Trim()
            };

            await _uow.Customers.AddAsync(customer);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Customer {CustomerId} created.", customer.Id);
            return customer;
        }

        public Task<Customer?> GetByIdAsync(Guid id, bool includeOrders = false)
            => _uow.Customers.GetByIdAsync(id, includeOrders);

        public Task<IReadOnlyList<Customer>> SearchAsync(string? term)
            => _uow.Customers.SearchAsync(term);
    }
}
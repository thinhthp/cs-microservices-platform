using DAL.Entities;
using System;

namespace DAL.Interfaces;

public interface IDealerRepository
{
    Task<IEnumerable<Dealer>> GetAllAsync();
    Task<Dealer?> GetByIdAsync(Guid id);
    Task<Dealer> AddAsync(Dealer dealer);
    Task UpdateAsync(Dealer dealer);
    Task DeleteAsync(Guid id);
}

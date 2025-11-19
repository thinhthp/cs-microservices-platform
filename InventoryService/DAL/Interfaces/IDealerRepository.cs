using DAL.Entities;

namespace DAL.Interfaces;

public interface IDealerRepository
{
    Task<IEnumerable<Dealer>> GetAllAsync();
    Task<Dealer?> GetByIdAsync(long id);
    Task<Dealer> AddAsync(Dealer dealer);
    Task UpdateAsync(Dealer dealer);
    Task DeleteAsync(long id);
}

using DAL.Entities.DbFirst;

namespace DAL.Interfaces;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> GetAllAsync();
    Task<Brand?> GetByIdAsync(long id);
    Task<Brand> AddAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(long id);
}

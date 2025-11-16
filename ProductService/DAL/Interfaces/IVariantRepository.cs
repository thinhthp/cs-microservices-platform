using DAL.Entities.DbFirst;

namespace DAL.Interfaces;

public interface IVariantRepository
{
    Task<IEnumerable<Variant>> GetAllAsync();
    Task<Variant?> GetByIdAsync(long id);
    Task<Variant> AddAsync(Variant variant);
    Task UpdateAsync(Variant variant);
    Task DeleteAsync(long id);
}

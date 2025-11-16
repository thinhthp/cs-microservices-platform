using DAL.Entities.DbFirst;

namespace DAL.Interfaces;

public interface IModelRepository
{
    Task<IEnumerable<Model>> GetAllAsync();
    Task<Model?> GetByIdAsync(long id);
    Task<Model> AddAsync(Model model);
    Task UpdateAsync(Model model);
    Task DeleteAsync(long id);
}

using DAL.Entities.DbFirst;

namespace DAL.Interfaces;

public interface IModelRepository
{
    Task<IEnumerable<Model>> GetAllAsync();
    Task<Model?> GetByIdAsync(Guid id);
    Task<Model> AddAsync(Model model);
    Task UpdateAsync(Model model);
    Task DeleteAsync(Guid id);
}

using BLL.DTOs.Model;

namespace BLL.Interfaces;

public interface IModelService
{
    Task<IEnumerable<ModelResponse>> GetAllAsync();
    Task<ModelResponse?> GetByIdAsync(Guid id);
    Task<ModelResponse> AddAsync(ModelRequest dto);
    Task UpdateAsync(Guid id, ModelRequest dto);
    Task DeleteAsync(Guid id);
}

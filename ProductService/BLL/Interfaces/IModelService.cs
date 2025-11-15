using BLL.DTOs.Model;

namespace BLL.Interfaces;

public interface IModelService
{
    Task<IEnumerable<ModelResponse>> GetAllAsync();
    Task<ModelResponse?> GetByIdAsync(long id);
    Task<ModelResponse> AddAsync(ModelRequest dto);
    Task UpdateAsync(long id, ModelRequest dto);
    Task DeleteAsync(long id);
}

using BLL.DTOs.Variant;

namespace BLL.Interfaces;

public interface IVariantService
{
    Task<IEnumerable<VariantResponse>> GetAllAsync();
    Task<VariantResponse?> GetByIdAsync(long id);
    Task<VariantResponse> AddAsync(VariantRequest dto);
    Task UpdateAsync(long id, VariantRequest dto);
    Task DeleteAsync(long id);
}

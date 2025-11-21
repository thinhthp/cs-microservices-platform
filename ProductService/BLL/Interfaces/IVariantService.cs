using BLL.DTOs.Variant;

namespace BLL.Interfaces;

public interface IVariantService
{
    Task<IEnumerable<VariantResponse>> GetAllAsync();
    Task<VariantResponse?> GetByIdAsync(Guid id);
    Task<VariantResponse> AddAsync(VariantRequest dto);
    Task UpdateAsync(Guid id, VariantRequest dto);
    Task DeleteAsync(Guid id);
}

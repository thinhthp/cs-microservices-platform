using BLL.DTOs.Brand;

namespace BLL.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandResponse>> GetAllAsync();
    Task<BrandResponse?> GetByIdAsync(Guid id);
    Task<BrandResponse> AddAsync(BrandRequest dto);
    Task UpdateAsync(Guid id, BrandRequest dto);
    Task DeleteAsync(Guid id);
}

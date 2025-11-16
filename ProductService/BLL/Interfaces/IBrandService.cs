using BLL.DTOs.Brand;

namespace BLL.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandResponse>> GetAllAsync();
    Task<BrandResponse?> GetByIdAsync(long id);
    Task<BrandResponse> AddAsync(BrandRequest dto);
    Task UpdateAsync(long id, BrandRequest dto);
    Task DeleteAsync(long id);
}

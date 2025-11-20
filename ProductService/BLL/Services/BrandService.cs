using BLL.DTOs.Brand;
using BLL.Interfaces;
using DAL.Entities.DbFirst;
using DAL.Interfaces;

namespace BLL.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<BrandResponse> AddAsync(BrandRequest dto)
    {
        var brand = new Brand { Name = dto.Name };

        var createdBrand = await _brandRepository.AddAsync(brand);
        return MapToDto(createdBrand);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _brandRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BrandResponse>> GetAllAsync()
    {
        var brands = await _brandRepository.GetAllAsync();
        return brands.Select(MapToDto);
    }

    public async Task<BrandResponse?> GetByIdAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        return brand != null ? MapToDto(brand) : null;
    }

    public async Task UpdateAsync(Guid id, BrandRequest dto)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            return;

        brand.Name = dto.Name;
        await _brandRepository.UpdateAsync(brand);
    }

    private BrandResponse MapToDto(Brand brand)
    {
        return new BrandResponse { Id = brand.Id, Name = brand.Name };
    }
}

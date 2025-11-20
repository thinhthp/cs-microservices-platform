using BLL.DTOs.Variant;
using BLL.Interfaces;
using DAL.Entities.DbFirst;
using DAL.Interfaces;

namespace BLL.Services;

public class VariantService : IVariantService
{
    private readonly IVariantRepository _variantRepository;

    public VariantService(IVariantRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public async Task<VariantResponse> AddAsync(VariantRequest dto)
    {
        var variant = new Variant
        {
            Name = dto.Name,
            RangeKm = dto.RangeKm,
            BasePrice = dto.BasePrice,
            ModelId = dto.ModelId,
        };
        var createdVariant = await _variantRepository.AddAsync(variant);
        return MapToDto(createdVariant);
    }

    public async Task DeleteAsync(long id)
    {
        await _variantRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<VariantResponse>> GetAllAsync()
    {
        var variants = await _variantRepository.GetAllAsync();
        return variants.Select(MapToDto);
    }

    public async Task<VariantResponse?> GetByIdAsync(long id)
    {
        var variant = await _variantRepository.GetByIdAsync(id);
        return variant == null ? null : MapToDto(variant);
    }

    public async Task UpdateAsync(long id, VariantRequest dto)
    {
        var variant = new Variant
        {
            Name = dto.Name,
            RangeKm = dto.RangeKm,
            BasePrice = dto.BasePrice,
            ModelId = dto.ModelId,
        };
        await _variantRepository.UpdateAsync(variant);
    }

    private VariantResponse MapToDto(Variant variant)
    {
        return new VariantResponse
        {
            Id = variant.Id,
            Name = variant.Name,
            RangeKm = variant.RangeKm,
            BasePrice = variant.BasePrice,
            ModelId = variant.ModelId,
        };
    }
}

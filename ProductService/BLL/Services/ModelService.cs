using BLL.DTOs.Model;
using BLL.Interfaces;
using DAL.Entities.DbFirst;
using DAL.Interfaces;

namespace BLL.Services;

public class ModelService : IModelService
{
    private readonly IModelRepository _modelRepository;

    public ModelService(IModelRepository modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<ModelResponse> AddAsync(ModelRequest dto)
    {
        var model = new Model { Name = dto.Name, BrandId = dto.BrandId };
        var createdModel = await _modelRepository.AddAsync(model);
        return MapToDto(createdModel);
    }

    public async Task DeleteAsync(long id)
    {
        await _modelRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ModelResponse>> GetAllAsync()
    {
        var models = await _modelRepository.GetAllAsync();
        return models.Select(MapToDto);
    }

    public async Task<ModelResponse?> GetByIdAsync(long id)
    {
        var model = await _modelRepository.GetByIdAsync(id);
        return model == null ? null : MapToDto(model);
    }

    public async Task UpdateAsync(long id, ModelRequest dto)
    {
        var model = new Model { Name = dto.Name, BrandId = dto.BrandId };
        await _modelRepository.UpdateAsync(model);
    }

    private ModelResponse MapToDto(Model model)
    {
        return new ModelResponse
        {
            Id = model.Id,
            Name = model.Name,
            BrandId = model.BrandId,
        };
    }
}

using BLL.DTOs.Model;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers;

[ApiController]
[Route("api/models")]
public class ModelsController : ControllerBase
{
    private readonly IModelService _modelService;

    public ModelsController(IModelService modelService)
    {
        _modelService = modelService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelResponse>>> GetAll()
    {
        var models = await _modelService.GetAllAsync();
        return Ok(models);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ModelResponse>> GetById(Guid id)
    {
        var model = await _modelService.GetByIdAsync(id);
        if (model == null)
            return NotFound();
        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<ModelResponse>> Create([FromBody] ModelRequest dto)
    {
        var createdModel = await _modelService.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdModel.Id }, createdModel);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ModelRequest dto)
    {
        var existingModel = await _modelService.GetByIdAsync(id);
        if (existingModel == null)
            return NotFound();
        await _modelService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingModel = await _modelService.GetByIdAsync(id);
        if (existingModel == null)
            return NotFound();
        await _modelService.DeleteAsync(id);
        return NoContent();
    }
}

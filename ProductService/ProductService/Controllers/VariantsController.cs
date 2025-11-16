using BLL.DTOs.Variant;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers;

[ApiController]
[Route("api/variants")]
public class VariantsController : ControllerBase
{
    private readonly IVariantService _variantService;

    public VariantsController(IVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VariantResponse>>> GetAll()
    {
        var items = await _variantService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<VariantResponse>> GetById(long id)
    {
        var item = await _variantService.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<VariantResponse>> Create([FromBody] VariantRequest dto)
    {
        var createdVariant = await _variantService.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdVariant.Id }, createdVariant);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] VariantRequest dto)
    {
        var existingVariant = await _variantService.GetByIdAsync(id);
        if (existingVariant == null)
            return NotFound();
        await _variantService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var existingVariant = await _variantService.GetByIdAsync(id);
        if (existingVariant == null)
            return NotFound();
        await _variantService.DeleteAsync(id);
        return NoContent();
    }
}

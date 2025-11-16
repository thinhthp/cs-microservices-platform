using BLL.DTOs.Brand;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResponse>>> GetAll()
    {
        var items = await _brandService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BrandResponse>> GetById(long id)
    {
        var item = await _brandService.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BrandResponse>> Create([FromBody] BrandRequest dto)
    {
        var createdBrand = await _brandService.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdBrand.Id }, createdBrand);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] BrandRequest dto)
    {
        var existingBrand = await _brandService.GetByIdAsync(id);
        if (existingBrand == null)
            return NotFound();
        await _brandService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var existingBrand = await _brandService.GetByIdAsync(id);
        if (existingBrand == null)
            return NotFound();

        await _brandService.DeleteAsync(id);
        return NoContent();
    }
}

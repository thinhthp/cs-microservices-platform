using BLL.DTOs.Dealer;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InventoryService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DealersController : ControllerBase
{
    private readonly IDealerService _dealerService;

    public DealersController(IDealerService dealerService)
    {
        _dealerService = dealerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dealers = await _dealerService.GetAllAsync();
        return Ok(dealers);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dealer = await _dealerService.GetByIdAsync(id);
        if (dealer == null)
        {
            return NotFound();
        }
        return Ok(dealer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DealerRequest dto)
    {
        var createdDealer = await _dealerService.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdDealer.Id }, createdDealer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DealerRequest dto)
    {
        var existingDealer = await _dealerService.GetByIdAsync(id);
        if (existingDealer == null)
            return NotFound();
        await _dealerService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingDealer = await _dealerService.GetByIdAsync(id);
        if (existingDealer == null)
            return NotFound();
        await _dealerService.DeleteAsync(id);
        return NoContent();
    }
}

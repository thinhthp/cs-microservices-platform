using BLL.DTOs.Inventory;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InventoryService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoriesController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoriesController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inventories = await _inventoryService.GetAllAsync();
        return Ok(inventories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }
        return Ok(inventory);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryRequest dto)
    {
        var createdInventory = await _inventoryService.AddAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdInventory.Id }, createdInventory);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] InventoryRequest dto)
    {
        var existingInventory = await _inventoryService.GetByIdAsync(id);
        if (existingInventory == null)
        {
            return NotFound();
        }
        await _inventoryService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingInventory = await _inventoryService.GetByIdAsync(id);
        if (existingInventory == null)
        {
            return NotFound();
        }
        await _inventoryService.DeleteAsync(id);
        return NoContent();
    }
}

using BLL.DTOs.Inventory;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryResponse> AddAsync(InventoryRequest dto)
    {
        var inventory = new Inventory
        {
            DealerId = dto.DealerId,
            VariantId = dto.VariantId,
            Quantity = dto.Quantity,
        };

        var createdInventory = await _inventoryRepository.AddAsync(inventory);
        return MapToDto(createdInventory);
    }

    public async Task DeleteAsync(long id)
    {
        await _inventoryRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<InventoryResponse>> GetAllAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        return inventories.Select(MapToDto);
    }

    public async Task<InventoryResponse?> GetByIdAsync(long id)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        return inventory != null ? MapToDto(inventory) : null;
    }

    public async Task UpdateAsync(long id, InventoryRequest dto)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return;

        inventory.DealerId = dto.DealerId;
        inventory.VariantId = dto.VariantId;
        inventory.Quantity = dto.Quantity;

        await _inventoryRepository.UpdateAsync(inventory);
    }

    private InventoryResponse MapToDto(Inventory inventory)
    {
        return new InventoryResponse
        {
            Id = inventory.Id,
            DealerId = inventory.DealerId,
            VariantId = inventory.VariantId,
            Quantity = inventory.Quantity,
        };
    }
}

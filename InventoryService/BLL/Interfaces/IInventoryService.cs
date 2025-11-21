using BLL.DTOs.Inventory;
using System;

namespace BLL.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryResponse>> GetAllAsync();
    Task<InventoryResponse?> GetByIdAsync(Guid id);
    Task<InventoryResponse> AddAsync(InventoryRequest dto);
    Task UpdateAsync(Guid id, InventoryRequest dto);
    Task DeleteAsync(Guid id);
}

using BLL.DTOs.Inventory;

namespace BLL.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryResponse>> GetAllAsync();
    Task<InventoryResponse?> GetByIdAsync(long id);
    Task<InventoryResponse> AddAsync(InventoryRequest dto);
    Task UpdateAsync(long id, InventoryRequest dto);
    Task DeleteAsync(long id);
}

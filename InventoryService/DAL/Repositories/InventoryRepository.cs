using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _context;

    public InventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory> AddAsync(Inventory inventory)
    {
        _context.Inventories.Add(inventory);
        await _context.SaveChangesAsync();
        return inventory;
    }

    public async Task DeleteAsync(long id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory != null)
        {
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync()
    {
        return await _context.Inventories.ToListAsync();
    }

    public async Task<Inventory?> GetByIdAsync(long id)
    {
        return await _context.Inventories.FindAsync(id);
    }

    public async Task UpdateAsync(Inventory inventory)
    {
        _context.Entry(inventory).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

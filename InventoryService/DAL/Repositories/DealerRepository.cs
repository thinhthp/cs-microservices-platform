using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class DealerRepository : IDealerRepository
{
    private readonly InventoryDbContext _context;

    public DealerRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Dealer> AddAsync(Dealer dealer)
    {
        _context.Dealers.Add(dealer);
        await _context.SaveChangesAsync();
        return dealer;
    }

    public async Task DeleteAsync(long id)
    {
        var dealer = await _context.Dealers.FindAsync(id);
        if (dealer != null)
        {
            _context.Dealers.Remove(dealer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Dealer>> GetAllAsync()
    {
        return await _context.Dealers.ToListAsync();
    }

    public async Task<Dealer?> GetByIdAsync(long id)
    {
        return await _context.Dealers.FindAsync(id);
    }

    public async Task UpdateAsync(Dealer dealer)
    {
        _context.Entry(dealer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

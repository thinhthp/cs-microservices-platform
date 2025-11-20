using DAL.Data.DbFirst;
using DAL.Entities.DbFirst;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class VariantRepository : IVariantRepository
{
    private readonly ProductDbFirstContext _context;

    public VariantRepository(ProductDbFirstContext context)
    {
        _context = context;
    }

    public async Task<Variant> AddAsync(Variant variant)
    {
        _context.Variants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task DeleteAsync(Guid id)
    {
        var variant = await _context.Variants.FindAsync(id);
        if (variant != null)
        {
            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Variant>> GetAllAsync()
    {
        return await _context.Variants.ToListAsync();
    }

    public async Task<Variant?> GetByIdAsync(Guid id)
    {
        return await _context.Variants.FindAsync(id);
    }

    public async Task UpdateAsync(Variant variant)
    {
        _context.Variants.Update(variant);
        await _context.SaveChangesAsync();
    }
}

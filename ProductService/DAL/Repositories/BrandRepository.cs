using DAL.Data.DbFirst;
using DAL.Entities.DbFirst;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly ProductDbFirstContext _context;

    public BrandRepository(ProductDbFirstContext context)
    {
        _context = context;
    }

    public async Task<Brand> AddAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task DeleteAsync(Guid id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand != null)
        {
            _context.Brands.Remove(brand);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands.ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(Guid id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task UpdateAsync(Brand brand)
    {
        _context.Entry(brand).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

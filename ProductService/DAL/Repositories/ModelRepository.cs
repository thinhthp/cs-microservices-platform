using DAL.Data.DbFirst;
using DAL.Entities.DbFirst;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class ModelRepository : IModelRepository
{
    private readonly ProductDbFirstContext _context;

    public ModelRepository(ProductDbFirstContext context)
    {
        _context = context;
    }

    public async Task<Model> AddAsync(Model model)
    {
        _context.Models.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task DeleteAsync(Guid id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model != null)
        {
            _context.Models.Remove(model);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Model>> GetAllAsync()
    {
        return await _context.Models.ToListAsync();
    }

    public async Task<Model?> GetByIdAsync(Guid id)
    {
        return await _context.Models.FindAsync(id);
    }

    public async Task UpdateAsync(Model model)
    {
        _context.Models.Update(model);
        await _context.SaveChangesAsync();
    }
}

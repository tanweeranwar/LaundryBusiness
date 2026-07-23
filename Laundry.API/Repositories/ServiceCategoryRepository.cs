using Laundry.API.Data;
using Laundry.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class ServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly LaundryDbContext _context;

    public ServiceCategoryRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceCategory>> GetAllAsync()
    {
        return await _context.ServiceCategories
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ServiceCategory?> GetByIdAsync(int id)
    {
        return await _context.ServiceCategories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ServiceCategory?> GetByNameAsync(string name)
    {
        return await _context.ServiceCategories
            .FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper());
    }

    public async Task AddAsync(ServiceCategory category)
    {
        await _context.ServiceCategories.AddAsync(category);
    }

    public Task UpdateAsync(ServiceCategory category)
    {
        _context.ServiceCategories.Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ServiceCategory category)
    {
        _context.ServiceCategories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
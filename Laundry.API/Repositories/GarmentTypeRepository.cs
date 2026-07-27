using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class GarmentTypeRepository : IGarmentTypeRepository
{
    private readonly LaundryDbContext _context;

    public GarmentTypeRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<List<GarmentType>> GetAllAsync()
    {
        return await _context.GarmentTypes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<GarmentType?> GetByIdAsync(int id)
    {
        return await _context.GarmentTypes
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<GarmentType?> GetByNameAsync(string name)
    {
        return await _context.GarmentTypes
            .FirstOrDefaultAsync(x =>
                x.Name.ToLower() == name.ToLower() &&
                x.IsActive);
    }

    public async Task<GarmentType> AddAsync(GarmentType garmentType)
    {
        await _context.GarmentTypes.AddAsync(garmentType);
        return garmentType;
    }

    public Task UpdateAsync(GarmentType garmentType)
    {
        _context.GarmentTypes.Update(garmentType);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(GarmentType garmentType)
    {
        garmentType.IsActive = false;
        _context.GarmentTypes.Update(garmentType);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.GarmentTypes
            .AnyAsync(x => x.Id == id && x.IsActive);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
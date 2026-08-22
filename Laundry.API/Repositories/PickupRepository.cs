using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class PickupRepository : IPickupRepository
{
    private readonly LaundryDbContext _context;

    public PickupRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<Pickup> AddAsync(Pickup pickup)
    {
        await _context.Pickups.AddAsync(pickup);
        return pickup;
    }

    public async Task<Pickup?> GetByIdAsync(int id)
    {
        return await _context.Pickups
            .AsNoTracking()
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Pickup?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Pickups
            .AsNoTracking()
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    public async Task<List<Pickup>> GetByStatusAsync(PickupStatus status)
    {
        return await _context.Pickups
            .AsNoTracking()
            .Include(x => x.Order)
            .Where(x => x.Status == status)
            .OrderBy(x => x.ScheduledDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Pickup pickup)
    {
        _context.Pickups.Update(pickup);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsForOrderAsync(int orderId)
    {
        return await _context.Pickups
            .AnyAsync(x => x.OrderId == orderId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
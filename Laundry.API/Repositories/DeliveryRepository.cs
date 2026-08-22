using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly LaundryDbContext _context;

    public DeliveryRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<Delivery> AddAsync(Delivery delivery)
    {
        await _context.Deliveries.AddAsync(delivery);
        return delivery;
    }

    public async Task<Delivery?> GetByIdAsync(int id)
    {
        return await _context.Deliveries
            .AsNoTracking()
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Delivery?> GetTrackedByIdAsync(int id)
    {
        return await _context.Deliveries
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Delivery?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Deliveries
            .AsNoTracking()
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    public async Task<List<Delivery>> GetByStatusAsync(
        DeliveryStatus status)
    {
        return await _context.Deliveries
            .AsNoTracking()
            .Include(x => x.Order)
            .Where(x => x.Status == status)
            .OrderBy(x => x.ScheduledDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Delivery delivery)
    {
        _context.Entry(delivery).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsForOrderAsync(int orderId)
    {
        return await _context.Deliveries
            .AnyAsync(x => x.OrderId == orderId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
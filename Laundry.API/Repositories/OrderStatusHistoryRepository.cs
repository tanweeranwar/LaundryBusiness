using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
{
    private readonly LaundryDbContext _context;

    public OrderStatusHistoryRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<OrderStatusHistory> AddAsync(
        OrderStatusHistory history)
    {
        await _context.OrderStatusHistories.AddAsync(history);

        return history;
    }

    public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(
        int orderId)
    {
        return await _context.OrderStatusHistories
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.ChangedOn)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly LaundryDbContext _context;

    public OrderRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<Order> AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber);
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _context.Orders
            .Include(x => x.Items)
                .ThenInclude(i => i.ServiceCategory)
            .Include(x => x.Items)
                .ThenInclude(i => i.GarmentType)
            .Include(x => x.Customer)
            .Include(x => x.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<List<Order>> GetOrdersByCustomerAsync(Guid customerId)
    {
        return await _context.Orders
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByBranchAsync(int branchId)
    {
        return await _context.Orders
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Order?> GetTrackedByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Orders
            .AnyAsync(x => x.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
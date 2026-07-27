using Laundry.API.Data;
using Laundry.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Services;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    private readonly LaundryDbContext _context;

    public OrderNumberGenerator(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync()
    {
        var today = DateTime.Now.Date;

        var todayCount = await _context.Orders
            .CountAsync(o => o.OrderDate.Date == today);

        var sequence = todayCount + 1;

        return $"ORD-{today:yyyyMMdd}-{sequence:D6}";
    }
}
using Laundry.API.Data;
using Microsoft.EntityFrameworkCore;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class PaymentNumberGenerator : IPaymentNumberGenerator
{
    private readonly LaundryDbContext _context;

    public PaymentNumberGenerator(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");

        var count = await _context.Payments
            .CountAsync(p => p.PaidOn.Date == DateTime.Now.Date);

        return $"PAY-{today}-{(count + 1):D6}";
    }
}
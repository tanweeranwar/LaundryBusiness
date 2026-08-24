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
        if (_context.Database.CurrentTransaction == null)
        {
            throw new InvalidOperationException(
                "Payment number generation must run inside a database transaction.");
        }

        // Serialize payment-number generation across application instances.
        // The transaction remains open until PaymentService commits the payment.
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT pg_advisory_xact_lock(hashtext('LaundryApp.PaymentNumber'));");

        var today = DateTime.Now.Date;
        var todayText = today.ToString("yyyyMMdd");

        var count = await _context.Payments
            .CountAsync(p => p.PaidOn.Date == today);

        return $"PAY-{todayText}-{(count + 1):D6}";
    }
}

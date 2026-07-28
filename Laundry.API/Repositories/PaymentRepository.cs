using Laundry.API.Data;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Laundry.API.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly LaundryDbContext _context;

    public PaymentRepository(LaundryDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Payment?> GetByPaymentNumberAsync(string paymentNumber)
    {
        return await _context.Payments
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.PaymentNumber == paymentNumber);
    }

    public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.PaidOn)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalPaidAsync(int orderId)
    {
        return await _context.Payments
            .Where(x =>
                x.OrderId == orderId &&
                x.PaymentStatus == PaymentStatus.Completed)
            .SumAsync(x => (decimal?)x.Amount) ?? 0m;
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
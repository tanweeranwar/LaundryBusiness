using Laundry.API.Entities;

namespace Laundry.API.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);

    Task<Payment?> GetByPaymentNumberAsync(string paymentNumber);

    Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);

    Task<decimal> GetTotalPaidAsync(int orderId);

    Task AddAsync(Payment payment);

    void Update(Payment payment);

    Task SaveChangesAsync();
}
using Laundry.API.DTOs.Payment;

namespace Laundry.API.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentDto request);

    Task<PaymentDto?> GetByIdAsync(int id);

    Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(int orderId);

    Task<PaymentDto> UpdateAsync(int id, UpdatePaymentDto request);
}
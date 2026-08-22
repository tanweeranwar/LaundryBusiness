using Laundry.API.DTOs.Delivery;

namespace Laundry.API.Services.Interfaces;

public interface IDeliveryService
{
    Task<DeliveryDto> CreateAsync(CreateDeliveryDto request);

    Task<DeliveryDto?> GetByIdAsync(int id);

    Task<DeliveryDto?> GetByOrderIdAsync(int orderId);

    Task<IEnumerable<DeliveryDto>> GetByStatusAsync(
        Laundry.API.Enums.DeliveryStatus status);

    Task<DeliveryDto> UpdateAsync(
        int id,
        UpdateDeliveryDto request);
}
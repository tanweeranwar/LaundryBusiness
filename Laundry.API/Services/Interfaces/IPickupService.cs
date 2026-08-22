using Laundry.API.DTOs.Pickup;

namespace Laundry.API.Services.Interfaces;

public interface IPickupService
{
    Task<PickupDto> CreateAsync(CreatePickupDto request);

    Task<PickupDto?> GetByIdAsync(int id);

    Task<PickupDto?> GetByOrderIdAsync(int orderId);

    Task<IEnumerable<PickupDto>> GetByStatusAsync(
        Laundry.API.Enums.PickupStatus status);

    Task<PickupDto> UpdateAsync(
        int id,
        UpdatePickupDto request);
}
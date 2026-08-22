using Laundry.API.Entities;
using Laundry.API.Enums;

namespace Laundry.API.Interfaces;

public interface IDeliveryRepository
{
    Task<Delivery> AddAsync(Delivery delivery);

    Task<Delivery?> GetByIdAsync(int id);

    Task<Delivery?> GetByOrderIdAsync(int orderId);

    Task<List<Delivery>> GetByStatusAsync(DeliveryStatus status);

    Task UpdateAsync(Delivery delivery);

    Task<bool> ExistsForOrderAsync(int orderId);

    Task SaveChangesAsync();
}
using Laundry.API.Entities;
using Laundry.API.Enums;

namespace Laundry.API.Interfaces;

public interface IPickupRepository
{
    Task<Pickup> AddAsync(Pickup pickup);

    Task<Pickup?> GetByIdAsync(int id);

    Task<Pickup?> GetByOrderIdAsync(int orderId);

    Task<List<Pickup>> GetByStatusAsync(PickupStatus status);

    Task UpdateAsync(Pickup pickup);

    Task<bool> ExistsForOrderAsync(int orderId);

    Task SaveChangesAsync();
}
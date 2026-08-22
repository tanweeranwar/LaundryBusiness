using Laundry.API.Entities;

namespace Laundry.API.Repositories.Interfaces;

public interface IOrderStatusHistoryRepository
{
    Task<OrderStatusHistory> AddAsync(OrderStatusHistory history);

    Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(int orderId);

    Task SaveChangesAsync();
}
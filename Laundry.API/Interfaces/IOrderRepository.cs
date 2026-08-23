using Laundry.API.Entities;
using Laundry.API.Enums;

namespace Laundry.API.Interfaces;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);

    Task<Order?> GetByIdAsync(int id);

    Task<Order?> GetByOrderNumberAsync(string orderNumber);

    Task<Order?> GetOrderWithItemsAsync(int orderId);

    Task<List<Order>> GetOrdersByCustomerAsync(
        Guid customerId);

    Task<List<Order>> GetOrdersByBranchAsync(
        int branchId);

    Task<List<Order>> GetOrdersByStatusAsync(
        OrderStatus status);

    Task<Order?> GetTrackedByIdAsync(int id);

    void Update(Order order);

    Task<bool> ExistsAsync(int id);

    Task SaveChangesAsync();
}
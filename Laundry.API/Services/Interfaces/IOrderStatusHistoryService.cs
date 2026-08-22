using Laundry.API.DTOs.Order;

namespace Laundry.API.Services.Interfaces;

public interface IOrderStatusHistoryService
{
    Task<IEnumerable<OrderStatusHistoryDto>> GetByOrderIdAsync(
        int orderId);
}
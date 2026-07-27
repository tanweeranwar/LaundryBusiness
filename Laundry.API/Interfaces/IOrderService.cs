using Laundry.API.DTOs.Orders;

namespace Laundry.API.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderDto request);

    Task<OrderDto?> GetByIdAsync(int id);

    Task<OrderDto?> GetByOrderNumberAsync(string orderNumber);

    Task<IEnumerable<OrderSummaryDto>> GetByCustomerAsync(Guid customerId);

    Task<IEnumerable<OrderSummaryDto>> GetByBranchAsync(int branchId);

    Task<bool> UpdateStatusAsync(int id, UpdateOrderDto request);
}
using Laundry.API.DTOs.Orders;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(
        CreateOrderDto request);

    Task<OrderDto?> GetByIdAsync(int id);

    Task<OrderDto?> GetByOrderNumberAsync(
        string orderNumber);

    Task<IEnumerable<OrderSummaryDto>> GetByCustomerAsync(
        Guid customerId);

    Task<IEnumerable<OrderSummaryDto>> GetByBranchAsync(
        int branchId);

    Task<bool> UpdateStatusAsync(
        int id,
        UpdateOrderDto request);

    Task<bool> MarkReadyAfterProcessingAsync(int orderId);
}
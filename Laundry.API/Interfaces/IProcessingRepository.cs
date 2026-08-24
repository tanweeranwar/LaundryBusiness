using Laundry.API.Entities;
using Laundry.API.Enums;

namespace Laundry.API.Interfaces;

public interface IProcessingRepository
{
    Task<OrderItem?> GetOrderItemWithProcessingAsync(int orderItemId);

    Task<Order?> GetOrderWithItemsAsync(int orderId);

    Task<ProcessingWorkflow?> GetWorkflowByServiceCategoryAsync(
        int serviceCategoryId);

    Task<OrderItemProcessing?> GetByIdAsync(int id);

    Task<OrderItemProcessing?> GetByOrderItemIdAsync(
        int orderItemId);

    Task<List<OrderItemProcessing>> GetByOrderIdAsync(
        int orderId);

    Task<OrderItemProcessing> AddAsync(
        OrderItemProcessing processing);

    Task UpdateAsync(
        OrderItemProcessing processing);

    Task<OrderItemProcessing?> GetForUpdateAsync(int processingId);

    Task<bool> AreAllOrderItemsProcessingCompletedAsync(int orderId);

    Task SaveChangesAsync();
}
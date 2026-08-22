using Laundry.API.DTOs.Order;
using Laundry.API.Repositories.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class OrderStatusHistoryService : IOrderStatusHistoryService
{
    private readonly IOrderStatusHistoryRepository
        _orderStatusHistoryRepository;

    public OrderStatusHistoryService(
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _orderStatusHistoryRepository =
            orderStatusHistoryRepository;
    }

    public async Task<IEnumerable<OrderStatusHistoryDto>>
        GetByOrderIdAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentException(
                "OrderId must be greater than zero.");

        var history =
            await _orderStatusHistoryRepository
                .GetByOrderIdAsync(orderId);

        return history.Select(x => new OrderStatusHistoryDto
        {
            Id = x.Id,
            OrderId = x.OrderId,
            FromStatus = x.FromStatus,
            ToStatus = x.ToStatus,
            Remarks = x.Remarks,
            ChangedBy = x.ChangedBy,
            ChangedOn = x.ChangedOn
        }).ToList();
    }
}
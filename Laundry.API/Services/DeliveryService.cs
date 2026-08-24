using Laundry.API.DTOs.Delivery;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        IOrderService orderService)
    {
        _deliveryRepository = deliveryRepository;
        _orderRepository = orderRepository;
        _orderService = orderService;
    }

    public async Task<DeliveryDto> CreateAsync(
        CreateDeliveryDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.OrderId <= 0)
            throw new ArgumentException(
                "OrderId must be greater than zero.");

        var order = await _orderRepository.GetTrackedByIdAsync(
            request.OrderId);

        if (order == null)
            throw new InvalidOperationException(
                $"Order '{request.OrderId}' does not exist.");

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Delivery cannot be created for a cancelled order.");
        }

        if (order.Status != OrderStatus.Ready)
        {
            throw new InvalidOperationException(
                $"Delivery can only be created for an order in Ready status. " +
                $"Current status is '{order.Status}'.");
        }

        if (await _deliveryRepository.ExistsForOrderAsync(
                request.OrderId))
        {
            throw new InvalidOperationException(
                $"A delivery already exists for order '{request.OrderId}'.");
        }

        ValidateScheduledDate(request.ScheduledDate);

        var delivery = new Delivery
        {
            OrderId = request.OrderId,
            Status = DeliveryStatus.Pending,
            ScheduledDate = NormalizeDate(request.ScheduledDate),
            AssignedTo = request.AssignedTo,
            Remarks = request.Remarks
        };

        await _deliveryRepository.AddAsync(delivery);
        await _deliveryRepository.SaveChangesAsync();

        return Map(delivery);
    }

    public async Task<DeliveryDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var delivery = await _deliveryRepository.GetByIdAsync(id);

        return delivery == null
            ? null
            : Map(delivery);
    }

    public async Task<DeliveryDto?> GetByOrderIdAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        var delivery =
            await _deliveryRepository.GetByOrderIdAsync(orderId);

        return delivery == null
            ? null
            : Map(delivery);
    }

    public async Task<IEnumerable<DeliveryDto>> GetByStatusAsync(
        DeliveryStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentException(
                "Invalid delivery status.");

        var deliveries =
            await _deliveryRepository.GetByStatusAsync(status);

        return deliveries
            .Select(Map)
            .ToList();
    }

    public async Task<DeliveryDto> UpdateAsync(
        int id,
        UpdateDeliveryDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (!Enum.IsDefined(request.Status))
            throw new ArgumentException(
                "Invalid delivery status.");

        var delivery =
            await _deliveryRepository.GetByIdAsync(id);

        if (delivery == null)
        {
            throw new KeyNotFoundException(
                $"Delivery '{id}' was not found.");
        }

        ValidateTransition(
            delivery.Status,
            request.Status);

        ValidateScheduledDate(request.ScheduledDate);

        var order =
            await _orderRepository.GetTrackedByIdAsync(
                delivery.OrderId);

        if (order == null)
        {
            throw new InvalidOperationException(
                $"Order '{delivery.OrderId}' does not exist.");
        }

        /*
         * Delivery status: Assigned
         * Order status: Ready -> OutForDelivery
         */
        if (request.Status == DeliveryStatus.OutForDelivery)
        {
            if (order.Status != OrderStatus.Ready)
            {
                throw new InvalidOperationException(
                    $"Order must be in Ready status before going " +
                    $"OutForDelivery. Current status is '{order.Status}'.");
            }

            await _orderService.UpdateStatusAsync(
                order.Id,
                new DTOs.Orders.UpdateOrderDto
                {
                    Status = (int)OrderStatus.OutForDelivery,
                    Remarks = "Delivery is out for delivery."
                });
        }

        /*
         * Delivery status: Delivered
         * Order status: OutForDelivery -> Delivered
         */
        if (request.Status == DeliveryStatus.Delivered)
        {
            if (order.Status != OrderStatus.OutForDelivery)
            {
                throw new InvalidOperationException(
                    $"Order must be in OutForDelivery status before " +
                    $"delivery can be completed. " +
                    $"Current status is '{order.Status}'.");
            }

            await _orderService.UpdateStatusAsync(
                order.Id,
                new DTOs.Orders.UpdateOrderDto
                {
                    Status = (int)OrderStatus.Delivered,
                    Remarks = "Delivery completed."
                });
        }

        delivery.Status = request.Status;
        delivery.ScheduledDate =
            NormalizeDate(request.ScheduledDate);

        delivery.AssignedTo =
            request.AssignedTo;

        delivery.Remarks =
            request.Remarks;

        if (request.Status == DeliveryStatus.Delivered)
        {
            delivery.DeliveredOn =
                NormalizeDate(
                    request.DeliveredOn ?? DateTime.Now);
        }
        else if (request.DeliveredOn.HasValue)
        {
            delivery.DeliveredOn =
                NormalizeDate(request.DeliveredOn);
        }

        delivery.UpdatedOn = DateTime.Now;

        await _deliveryRepository.UpdateAsync(delivery);
        await _deliveryRepository.SaveChangesAsync();

        return Map(delivery);
    }

    private static void ValidateTransition(
        DeliveryStatus current,
        DeliveryStatus requested)
    {
        if (current == requested)
            return;

        if (current == DeliveryStatus.Cancelled ||
            current == DeliveryStatus.Delivered)
        {
            throw new InvalidOperationException(
                $"Delivery cannot transition from '{current}' " +
                $"to '{requested}'.");
        }

        var valid = current switch
        {
            DeliveryStatus.Pending =>
                requested == DeliveryStatus.Scheduled ||
                requested == DeliveryStatus.Cancelled,

            DeliveryStatus.Scheduled =>
                requested == DeliveryStatus.Assigned ||
                requested == DeliveryStatus.Cancelled,

            DeliveryStatus.Assigned =>
                requested == DeliveryStatus.OutForDelivery ||
                requested == DeliveryStatus.Failed ||
                requested == DeliveryStatus.Cancelled,

            DeliveryStatus.OutForDelivery =>
                requested == DeliveryStatus.Delivered ||
                requested == DeliveryStatus.Failed,

            DeliveryStatus.Failed =>
                requested == DeliveryStatus.Scheduled ||
                requested == DeliveryStatus.Assigned ||
                requested == DeliveryStatus.Cancelled,

            _ => false
        };

        if (!valid)
        {
            throw new InvalidOperationException(
                $"Invalid delivery transition: " +
                $"{current} -> {requested}.");
        }
    }

    private static void ValidateScheduledDate(
        DateTime? date)
    {
        if (date.HasValue &&
            date.Value.Date < DateTime.Today)
        {
            throw new ArgumentException(
                "Scheduled date cannot be in the past.");
        }
    }

    private static DateTime? NormalizeDate(
        DateTime? value)
    {
        return value.HasValue
            ? DateTime.SpecifyKind(
                value.Value,
                DateTimeKind.Unspecified)
            : null;
    }

    private static DeliveryDto Map(
        Delivery delivery)
    {
        return new DeliveryDto
        {
            Id = delivery.Id,
            OrderId = delivery.OrderId,
            Status = delivery.Status,
            ScheduledDate = delivery.ScheduledDate,
            DeliveredOn = delivery.DeliveredOn,
            AssignedTo = delivery.AssignedTo,
            Remarks = delivery.Remarks
        };
    }
}
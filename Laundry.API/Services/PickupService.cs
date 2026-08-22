using Laundry.API.DTOs.Pickup;
using Laundry.API.Entities;
using Laundry.API.Enums;
using Laundry.API.Interfaces;
using Laundry.API.Services.Interfaces;

namespace Laundry.API.Services;

public class PickupService : IPickupService
{
    private readonly IPickupRepository _pickupRepository;
    private readonly IOrderRepository _orderRepository;

    public PickupService(
        IPickupRepository pickupRepository,
        IOrderRepository orderRepository)
    {
        _pickupRepository = pickupRepository;
        _orderRepository = orderRepository;
    }

    public async Task<PickupDto> CreateAsync(CreatePickupDto request)
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

        if (order.Status == OrderStatus.Cancelled ||
            order.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                "Pickup cannot be created for a cancelled or delivered order.");
        }

        if (await _pickupRepository.ExistsForOrderAsync(request.OrderId))
        {
            throw new InvalidOperationException(
                $"A pickup already exists for order '{request.OrderId}'.");
        }

        ValidateScheduledDate(request.ScheduledDate);

        var pickup = new Pickup
        {
            OrderId = request.OrderId,
            Status = PickupStatus.Pending,
            ScheduledDate = NormalizeDate(request.ScheduledDate),
            AssignedTo = request.AssignedTo,
            Remarks = request.Remarks
        };

        await _pickupRepository.AddAsync(pickup);
        await _pickupRepository.SaveChangesAsync();

        return Map(pickup);
    }

    public async Task<PickupDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        var pickup = await _pickupRepository.GetByIdAsync(id);

        return pickup == null ? null : Map(pickup);
    }

    public async Task<PickupDto?> GetByOrderIdAsync(int orderId)
    {
        if (orderId <= 0)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        var pickup = await _pickupRepository.GetByOrderIdAsync(orderId);

        return pickup == null ? null : Map(pickup);
    }

    public async Task<IEnumerable<PickupDto>> GetByStatusAsync(
        PickupStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new ArgumentException(
                "Invalid pickup status.");

        var pickups = await _pickupRepository.GetByStatusAsync(status);

        return pickups.Select(Map).ToList();
    }

    public async Task<PickupDto> UpdateAsync(
        int id,
        UpdatePickupDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (!Enum.IsDefined(request.Status))
            throw new ArgumentException(
                "Invalid pickup status.");

        var pickup = await _pickupRepository.GetByIdAsync(id);

        if (pickup == null)
            throw new KeyNotFoundException(
                $"Pickup '{id}' was not found.");

        ValidateTransition(
            pickup.Status,
            request.Status);

        ValidateScheduledDate(request.ScheduledDate);

        pickup.Status = request.Status;
        pickup.ScheduledDate = NormalizeDate(request.ScheduledDate);
        pickup.AssignedTo = request.AssignedTo;
        pickup.Remarks = request.Remarks;

        if (request.Status == PickupStatus.PickedUp)
        {
            pickup.PickedUpOn = NormalizeDate(
                request.PickedUpOn ?? DateTime.Now);
        }
        else if (request.PickedUpOn.HasValue)
        {
            pickup.PickedUpOn = NormalizeDate(request.PickedUpOn);
        }

        pickup.UpdatedOn = DateTime.Now;

        await _pickupRepository.UpdateAsync(pickup);
        await _pickupRepository.SaveChangesAsync();

        return Map(pickup);
    }

    private static void ValidateTransition(
        PickupStatus current,
        PickupStatus requested)
    {
        if (current == requested)
            return;

        if (current == PickupStatus.Cancelled ||
            current == PickupStatus.PickedUp ||
            current == PickupStatus.Failed)
        {
            throw new InvalidOperationException(
                $"Pickup cannot transition from '{current}' to '{requested}'.");
        }

        var valid = current switch
        {
            PickupStatus.Pending =>
                requested == PickupStatus.Scheduled ||
                requested == PickupStatus.Cancelled,

            PickupStatus.Scheduled =>
                requested == PickupStatus.Assigned ||
                requested == PickupStatus.PickedUp ||
                requested == PickupStatus.Failed ||
                requested == PickupStatus.Cancelled,

            PickupStatus.Assigned =>
                requested == PickupStatus.PickedUp ||
                requested == PickupStatus.Failed ||
                requested == PickupStatus.Cancelled,

            _ => false
        };

        if (!valid)
        {
            throw new InvalidOperationException(
                $"Invalid pickup transition: {current} -> {requested}.");
        }
    }

    private static void ValidateScheduledDate(DateTime? date)
    {
        if (date.HasValue &&
            date.Value.Date < DateTime.Today)
        {
            throw new ArgumentException(
                "Scheduled date cannot be in the past.");
        }
    }

    private static DateTime? NormalizeDate(DateTime? value)
    {
        return value.HasValue
            ? DateTime.SpecifyKind(
                value.Value,
                DateTimeKind.Unspecified)
            : null;
    }

    private static PickupDto Map(Pickup pickup)
    {
        return new PickupDto
        {
            Id = pickup.Id,
            OrderId = pickup.OrderId,
            Status = pickup.Status,
            ScheduledDate = pickup.ScheduledDate,
            PickedUpOn = pickup.PickedUpOn,
            AssignedTo = pickup.AssignedTo,
            Remarks = pickup.Remarks
        };
    }
}
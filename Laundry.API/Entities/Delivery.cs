using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class Delivery : BaseEntity
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

    public DateTime? ScheduledDate { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }

    public Order Order { get; set; } = null!;
}
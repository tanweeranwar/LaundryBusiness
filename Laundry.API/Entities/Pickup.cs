using Laundry.API.Enums;

namespace Laundry.API.Entities;

public class Pickup : BaseEntity
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public PickupStatus Status { get; set; } = PickupStatus.Pending;

    public DateTime? ScheduledDate { get; set; }

    public DateTime? PickedUpOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }

    public Order Order { get; set; } = null!;
}
using Laundry.API.Enums;

namespace Laundry.API.DTOs.Pickup;

public class PickupDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public PickupStatus Status { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public DateTime? PickedUpOn { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}
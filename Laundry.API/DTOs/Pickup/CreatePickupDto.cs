namespace Laundry.API.DTOs.Pickup;

public class CreatePickupDto
{
    public int OrderId { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}
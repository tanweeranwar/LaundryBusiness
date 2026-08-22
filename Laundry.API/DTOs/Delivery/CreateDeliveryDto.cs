namespace Laundry.API.DTOs.Delivery;

public class CreateDeliveryDto
{
    public int OrderId { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public string? AssignedTo { get; set; }

    public string? Remarks { get; set; }
}